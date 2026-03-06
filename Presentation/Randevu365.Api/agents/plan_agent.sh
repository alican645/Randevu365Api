#!/bin/bash
#
# Plan Agent - Gemini CLI ile codebase analizi yapip detayli plan dosyasi uretir
#
# Kullanim:
#   ./agents/plan_agent.sh "Kullaniciya bildirim gonderme ozelligi ekle"
#   ./agents/plan_agent.sh "Siparis ekranina arama filtresi ekle"
#
# Cikti: plans/ klasorune tarih damgali .md dosyasi

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
PLANS_DIR="$PROJECT_ROOT/plans"
PROMPT_FILE="$SCRIPT_DIR/plan_agent_prompt.md"

# Renk kodlari
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

# Parametre kontrolu
if [ $# -eq 0 ]; then
  echo -e "${RED}Hata: Ozellik tanimi belirtmelisiniz.${NC}"
  echo ""
  echo "Kullanim:"
  echo "  ./agents/plan_agent.sh \"Ozellik tanimi burada\""
  echo ""
  echo "Ornekler:"
  echo "  ./agents/plan_agent.sh \"Siparis ekranina urun arama filtresi ekle\""
  echo "  ./agents/plan_agent.sh \"Dark mode destegi ekle\""
  exit 1
fi

FEATURE_DESC="$*"
TIMESTAMP=$(date +"%Y%m%d_%H%M")
OUTPUT_FILE="$PLANS_DIR/${TIMESTAMP}.md"

# Plans klasorunu olustur
mkdir -p "$PLANS_DIR"

echo -e "${GREEN}Plan Agent baslatiliyor...${NC}"
echo -e "${YELLOW}Ozellik:${NC} $FEATURE_DESC"
echo -e "${YELLOW}Cikti:${NC}   $OUTPUT_FILE"
echo ""

# Proje yapisini topla
echo -e "${GREEN}Proje yapisi analiz ediliyor...${NC}"
PROJECT_TREE=$(find "$PROJECT_ROOT/lib" -name "*.dart" -type f | sort | sed "s|$PROJECT_ROOT/||")

# System prompt'u oku
SYSTEM_PROMPT=$(cat "$PROMPT_FILE")

# Gemini'ye gonderilecek prompt
FULL_PROMPT=$(cat <<PROMPT_END
$SYSTEM_PROMPT

---

## Proje Dosya Yapisi

\`\`\`
$PROJECT_TREE
\`\`\`

## Proje Koku: $PROJECT_ROOT

## Istenen Ozellik

$FEATURE_DESC

---

Simdi yukaridaki talimatlara gore detayli bir plan dosyasi uret. Ciktini SADECE markdown formatinda ver, baska aciklama ekleme.
PROMPT_END
)

echo -e "${GREEN}Gemini CLI calistiriliyor...${NC}"
echo ""

# Gemini CLI'yi calistir
cd "$PROJECT_ROOT"

# Prompt'u gecici dosyaya yaz (cok uzun prompt'lar icin)
TEMP_PROMPT=$(mktemp)
echo "$FULL_PROMPT" > "$TEMP_PROMPT"

cat "$TEMP_PROMPT" | gemini \
  --prompt "" \
  --output-format text \
  --yolo \
  > "$OUTPUT_FILE" 2>/tmp/gemini_plan_agent_err.log || {
    echo -e "${RED}Gemini CLI hatasi!${NC}"
    echo "Hata detayi:"
    cat /tmp/gemini_plan_agent_err.log
    rm -f "$OUTPUT_FILE" "$TEMP_PROMPT"
    exit 1
  }

rm -f "$TEMP_PROMPT"

# Cikti kontrolu
if [ ! -s "$OUTPUT_FILE" ]; then
  echo -e "${RED}Gemini bos cikti uretti!${NC}"
  rm -f "$OUTPUT_FILE"
  exit 1
fi

echo ""
echo -e "${GREEN}Plan dosyasi basariyla olusturuldu!${NC}"
echo -e "${YELLOW}Dosya:${NC} $OUTPUT_FILE"
echo ""
echo "--- Plan Ozeti ---"
head -30 "$OUTPUT_FILE"
echo ""
echo "--- (devami icin dosyayi acin) ---"
