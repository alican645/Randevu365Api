#!/bin/bash
#
# Multi Planner Agent - Birden fazla Gemini CLI instance ile paralel plan uretici
#
# Calisma Akisi:
#   1. Orchestrator: Problemi analiz eder, alt gorevlere boler
#   2. Workers: Her alt gorev icin paralel Gemini CLI calistirir
#   3. Merger: Tum worker ciktilarini birlestirip final plan uretir
#
# Kullanim:
#   ./agents/multi_planner_agent.sh "Randevu iptal ve yeniden planlama ozelligi ekle"
#   ./agents/multi_planner_agent.sh "Isletme paneline istatistik dashboard ekle"
#
# Cikti: plans/ klasorune tarih damgali .md dosyasi

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
PLANS_DIR="$PROJECT_ROOT/plans"
SHARED_CTX_DIR=""

# Prompt dosyalari
ORCHESTRATOR_PROMPT="$SCRIPT_DIR/multi_planner_prompt.md"
WORKER_PROMPT="$SCRIPT_DIR/multi_planner_worker_prompt.md"
MERGER_PROMPT="$SCRIPT_DIR/multi_planner_merger_prompt.md"

# Renk kodlari
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
BOLD='\033[1m'
NC='\033[0m'

# Temizlik fonksiyonu
cleanup() {
  if [ -n "$SHARED_CTX_DIR" ] && [ -d "$SHARED_CTX_DIR" ]; then
    rm -rf "$SHARED_CTX_DIR"
  fi
}
trap cleanup EXIT

# Parametre kontrolu
if [ $# -eq 0 ]; then
  echo -e "${RED}Hata: Problem/ozellik tanimi belirtmelisiniz.${NC}"
  echo ""
  echo -e "${BOLD}Kullanim:${NC}"
  echo "  ./agents/multi_planner_agent.sh \"Problem tanimi burada\""
  echo ""
  echo -e "${BOLD}Ornekler:${NC}"
  echo "  ./agents/multi_planner_agent.sh \"Randevu iptal ve yeniden planlama ozelligi\""
  echo "  ./agents/multi_planner_agent.sh \"Isletme paneline istatistik dashboard ekle\""
  echo "  ./agents/multi_planner_agent.sh \"Bildirim sistemi entegre et\""
  exit 1
fi

FEATURE_DESC="$*"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
OUTPUT_FILE="$PLANS_DIR/${TIMESTAMP}.md"

# Gerekli dizinleri olustur
mkdir -p "$PLANS_DIR"

# Ortak context dizini (gecici)
SHARED_CTX_DIR=$(mktemp -d)
mkdir -p "$SHARED_CTX_DIR/workers"

echo -e "${CYAN}========================================================${NC}"
echo -e "${CYAN}  Multi Planner Agent${NC}"
echo -e "${CYAN}========================================================${NC}"
echo ""
echo -e "${YELLOW}Problem:${NC}  $FEATURE_DESC"
echo -e "${YELLOW}Cikti:${NC}    $OUTPUT_FILE"
echo -e "${YELLOW}Context:${NC}  $SHARED_CTX_DIR"
echo ""

# ================================================================
# FAZA 1: Proje yapisini topla ve ortak context'e yaz
# ================================================================
echo -e "${GREEN}[Faz 1/4] Proje yapisi toplaniliyor...${NC}"

PROJECT_TREE=$(find "$PROJECT_ROOT/lib" -name "*.dart" -type f 2>/dev/null | sort | sed "s|$PROJECT_ROOT/||")

cat > "$SHARED_CTX_DIR/project_context.md" <<CTX_END
# Proje Ortak Context

## Problem / Ozellik Talebi
$FEATURE_DESC

## Proje Koku
$PROJECT_ROOT

## Dosya Yapisi
\`\`\`
$PROJECT_TREE
\`\`\`

## Proje Mimarisi
- **Framework**: Flutter/Dart
- **State Management**: flutter_bloc (BLoC pattern)
- **Routing**: GoRouter
- **API**: REST API
- **Yapi**: Feature-based folder structure
  - \`lib/core/domain/\` - Entity'ler ve interface'ler
  - \`lib/core/constants/\` - Sabitler
  - \`lib/infrastructure/\` - API implementation
  - \`lib/presentation/\` - Ekranlar, navigation, widgets
CTX_END

echo -e "  Proje context dosyasi olusturuldu."

# ================================================================
# FAZ 2: Orchestrator - Problemi alt gorevlere bol
# ================================================================
echo ""
echo -e "${GREEN}[Faz 2/4] Orchestrator calistiriliyor - problem analiz ediliyor...${NC}"

# Orchestrator prompt'u dosyaya yaz (heredoc yerine - ozel karakter sorunu onlenir)
ORCH_TEMP=$(mktemp)
{
  cat "$ORCHESTRATOR_PROMPT"
  echo ""
  echo "---"
  echo ""
  cat "$SHARED_CTX_DIR/project_context.md"
  echo ""
  echo "---"
  echo ""
  echo "Simdi yukaridaki problemi analiz et, ilgili dosyalari oku ve alt gorevlere bol. Ciktini SADECE JSON formatinda ver."
} > "$ORCH_TEMP"

ORCH_OUTPUT="$SHARED_CTX_DIR/orchestrator_output.json"

cd "$PROJECT_ROOT"
cat "$ORCH_TEMP" | gemini \
  --prompt "" \
  --output-format text \
  --yolo \
  > "$ORCH_OUTPUT" 2>/tmp/gemini_multi_orch_err.log || {
    echo -e "${RED}Orchestrator hatasi!${NC}"
    cat /tmp/gemini_multi_orch_err.log
    rm -f "$ORCH_TEMP"
    exit 1
  }

rm -f "$ORCH_TEMP"

# JSON'dan code block isaretlerini temizle
sed -i '' 's/^```json//g; s/^```//g' "$ORCH_OUTPUT" 2>/dev/null || true

# JSON gecerliligi kontrol et
if ! python3 -c "import json; json.load(open('$ORCH_OUTPUT'))" 2>/dev/null; then
  # JSON satirlarini cikarmaya calis
  python3 -c "
import re, json, sys
content = open('$ORCH_OUTPUT').read()
# JSON blogu bul
match = re.search(r'\{.*\}', content, re.DOTALL)
if match:
    try:
        data = json.loads(match.group())
        json.dump(data, open('$ORCH_OUTPUT', 'w'), ensure_ascii=False, indent=2)
    except:
        print('JSON parse hatasi', file=sys.stderr)
        sys.exit(1)
else:
    print('JSON bulunamadi', file=sys.stderr)
    sys.exit(1)
" 2>/tmp/gemini_json_fix_err.log || {
    echo -e "${RED}Orchestrator gecersiz JSON uretti!${NC}"
    echo "Ham cikti:"
    cat "$ORCH_OUTPUT"
    exit 1
  }
fi

# Alt gorev sayisini al
TASK_COUNT=$(python3 -c "import json; data=json.load(open('$ORCH_OUTPUT')); print(len(data.get('sub_tasks', [])))")
SHARED_CONTEXT=$(python3 -c "import json; data=json.load(open('$ORCH_OUTPUT')); print(data.get('shared_context', ''))")

echo -e "  ${MAGENTA}$TASK_COUNT alt gorev belirlendi.${NC}"

# Ortak context'i dosyaya yaz
echo "$SHARED_CONTEXT" > "$SHARED_CTX_DIR/shared_context.txt"

# Her alt gorevi ayri dosyaya yaz
python3 -c "
import json
data = json.load(open('$ORCH_OUTPUT'))
for i, task in enumerate(data.get('sub_tasks', [])):
    with open('$SHARED_CTX_DIR/workers/task_{}.json'.format(i), 'w') as f:
        json.dump(task, f, ensure_ascii=False, indent=2)
    print('  Alt gorev {}: {}'.format(i+1, task.get('title', 'N/A')))
"

# ================================================================
# FAZ 3: Worker Agent'lar - Paralel Gemini CLI
# ================================================================
echo ""
echo -e "${GREEN}[Faz 3/4] Worker agent'lar paralel olarak baslatiliyor...${NC}"

WORKER_SYS=$(cat "$WORKER_PROMPT")
WORKER_PIDS=()

for i in $(seq 0 $((TASK_COUNT - 1))); do
  TASK_FILE="$SHARED_CTX_DIR/workers/task_${i}.json"
  WORKER_OUTPUT="$SHARED_CTX_DIR/workers/output_${i}.md"

  # Alt gorev bilgilerini oku
  TASK_TITLE=$(python3 -c "import json; print(json.load(open('$TASK_FILE')).get('title', 'Gorev $((i+1))'))")
  TASK_DESC=$(python3 -c "import json; print(json.load(open('$TASK_FILE')).get('description', ''))")
  TASK_FOCUS=$(python3 -c "import json; print('\n'.join(json.load(open('$TASK_FILE')).get('focus_areas', [])))")

  echo -e "  ${MAGENTA}Worker $((i+1)):${NC} $TASK_TITLE"

  # Worker prompt'u dosyaya yaz (heredoc yerine - ozel karakter sorunu onlenir)
  WORKER_TEMP=$(mktemp)
  {
    cat "$WORKER_PROMPT"
    echo ""
    echo "---"
    echo ""
    echo "## Ortak Context"
    echo ""
    cat "$SHARED_CTX_DIR/project_context.md"
    echo ""
    echo "### Paylasilan Bilgi"
    cat "$SHARED_CTX_DIR/shared_context.txt"
    echo ""
    echo "---"
    echo ""
    echo "## Sana Atanan Alt Gorev"
    echo ""
    echo "**Baslik:** $TASK_TITLE"
    echo ""
    echo "**Aciklama:** $TASK_DESC"
    echo ""
    echo "**Odak Alanlari:**"
    echo "$TASK_FOCUS"
    echo ""
    echo "---"
    echo ""
    echo "Simdi yukaridaki alt gorevi derinlemesine arastir. Ilgili dosyalari oku, analiz et ve detayli bulgularini markdown formatinda yaz."
  } > "$WORKER_TEMP"

  (
    cd "$PROJECT_ROOT"
    cat "$WORKER_TEMP" | gemini \
      --prompt "" \
      --output-format text \
      --yolo \
      > "$WORKER_OUTPUT" 2>/tmp/gemini_worker_${i}_err.log || true
    rm -f "$WORKER_TEMP"
  ) &

  WORKER_PIDS+=($!)
done

# Tum worker'larin bitmesini bekle
echo ""
echo -e "${YELLOW}  Tum worker'lar calisiyor, bekleniyor...${NC}"

FAILED_WORKERS=0
for i in "${!WORKER_PIDS[@]}"; do
  PID=${WORKER_PIDS[$i]}
  if wait "$PID"; then
    echo -e "  ${GREEN}Worker $((i+1)) tamamlandi.${NC}"
  else
    echo -e "  ${RED}Worker $((i+1)) basarisiz!${NC}"
    FAILED_WORKERS=$((FAILED_WORKERS + 1))
  fi
done

# Bos ciktilari kontrol et
VALID_OUTPUTS=0
for i in $(seq 0 $((TASK_COUNT - 1))); do
  OUTPUT_F="$SHARED_CTX_DIR/workers/output_${i}.md"
  if [ -s "$OUTPUT_F" ]; then
    VALID_OUTPUTS=$((VALID_OUTPUTS + 1))
  else
    echo -e "  ${RED}Worker $((i+1)) bos cikti uretti!${NC}"
  fi
done

if [ "$VALID_OUTPUTS" -eq 0 ]; then
  echo -e "${RED}Hicbir worker gecerli cikti uretmedi! Iptal ediliyor.${NC}"
  exit 1
fi

echo -e "  ${GREEN}$VALID_OUTPUTS/$TASK_COUNT worker basarili.${NC}"

# ================================================================
# FAZ 4: Merger - Tum ciktilari birlestir
# ================================================================
echo ""
echo -e "${GREEN}[Faz 4/4] Merger calistiriliyor - plan birlestiriliyor...${NC}"

# Merger prompt'u dosyaya yaz (heredoc yerine - ozel karakter sorunu onlenir)
MERGER_TEMP=$(mktemp)
{
  cat "$MERGER_PROMPT"
  echo ""
  echo "---"
  echo ""
  echo "## Orijinal Problem"
  echo "$FEATURE_DESC"
  echo ""
  echo "## Proje Context"
  cat "$SHARED_CTX_DIR/project_context.md"
  echo ""
  echo "## Ortak Bilgi"
  cat "$SHARED_CTX_DIR/shared_context.txt"
  echo ""
  echo "## Worker Agent Ciktilari"
  echo ""
  echo "Asagida $VALID_OUTPUTS worker agentin urettigi analizler var. Bunlari tutarli bir plan olarak birlestir."
  echo ""

  for i in $(seq 0 $((TASK_COUNT - 1))); do
    OUTPUT_F="$SHARED_CTX_DIR/workers/output_${i}.md"
    if [ -s "$OUTPUT_F" ]; then
      TASK_TITLE=$(python3 -c "import json; print(json.load(open('$SHARED_CTX_DIR/workers/task_${i}.json')).get('title', 'Gorev $((i+1))'))")
      echo ""
      echo "---"
      echo ""
      echo "### Worker $((i+1)): $TASK_TITLE"
      echo ""
      cat "$OUTPUT_F"
      echo ""
    fi
  done

  echo ""
  echo "---"
  echo ""
  echo "Simdi yukaridaki tum worker ciktilarini birlestirerek kapsamli, tutarli ve uygulamaya hazir bir plan uret. Ciktini SADECE markdown formatinda ver."
  echo ""
  echo "Planin basina su bilgileri ekle:"
  echo "- Olusturulma: $TIMESTAMP"
  echo "- Agent Sayisi: $VALID_OUTPUTS"
  echo "- Problem: $FEATURE_DESC"
} > "$MERGER_TEMP"

cd "$PROJECT_ROOT"
cat "$MERGER_TEMP" | gemini \
  --prompt "" \
  --output-format text \
  --yolo \
  > "$OUTPUT_FILE" 2>/tmp/gemini_merger_err.log || {
    echo -e "${RED}Merger hatasi!${NC}"
    cat /tmp/gemini_merger_err.log
    rm -f "$MERGER_TEMP"
    exit 1
  }

rm -f "$MERGER_TEMP"

# Cikti kontrolu
if [ ! -s "$OUTPUT_FILE" ]; then
  echo -e "${RED}Merger bos cikti uretti!${NC}"
  rm -f "$OUTPUT_FILE"
  exit 1
fi

# ================================================================
# SONUC
# ================================================================
echo ""
echo -e "${CYAN}========================================================${NC}"
echo -e "${CYAN}  Multi Planner Agent tamamlandi!${NC}"
echo -e "${CYAN}========================================================${NC}"
echo ""
echo -e "${YELLOW}Plan dosyasi:${NC}    $OUTPUT_FILE"
echo -e "${YELLOW}Worker sayisi:${NC}   $VALID_OUTPUTS/$TASK_COUNT"
echo -e "${YELLOW}Basarisiz:${NC}       $FAILED_WORKERS"
echo ""
echo -e "${BOLD}--- Plan Ozeti ---${NC}"
head -40 "$OUTPUT_FILE"
echo ""
echo -e "${BOLD}--- (devami icin: cat $OUTPUT_FILE) ---${NC}"
