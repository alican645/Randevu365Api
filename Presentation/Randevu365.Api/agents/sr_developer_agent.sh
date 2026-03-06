#!/bin/bash
#
# SR Developer Agent - Claude CLI ile plan dosyasini implemente eder
#
# Kullanim:
#   ./agents/sr_developer_agent.sh plans/20260306_siparis_filtre.md
#   ./agents/sr_developer_agent.sh plans/latest.md --dry-run
#
# Ozellikler:
#   - Claude CLI kullanir (acceptEdits modu)
#   - Calisma oncesi /compact yapar
#   - Plan dosyasini okuyup adim adim uygular

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
PROMPT_FILE="$SCRIPT_DIR/sr_developer_prompt.md"

# Renk kodlari
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
CYAN='\033[0;36m'
NC='\033[0m'

# Parametre kontrolu
if [ $# -eq 0 ]; then
  echo -e "${RED}Hata: Plan dosyasi belirtmelisiniz.${NC}"
  echo ""
  echo "Kullanim:"
  echo "  ./agents/sr_developer_agent.sh <plan-dosyasi.md>"
  echo ""
  echo "Ornekler:"
  echo "  ./agents/sr_developer_agent.sh plans/20260306_siparis_filtre.md"
  echo ""
  echo "Mevcut planlar:"
  if [ -d "$PROJECT_ROOT/plans" ]; then
    ls -1t "$PROJECT_ROOT/plans/"*.md 2>/dev/null | head -10 | while read f; do
      echo "  $(basename "$f")"
    done
  else
    echo "  (plans/ klasoru bos)"
  fi
  exit 1
fi

PLAN_FILE="$1"

# Dosya yolunu duzelt (relative ise absolute yap)
if [[ "$PLAN_FILE" != /* ]]; then
  PLAN_FILE="$PROJECT_ROOT/$PLAN_FILE"
fi

# Plan dosyasi var mi kontrol et
if [ ! -f "$PLAN_FILE" ]; then
  echo -e "${RED}Hata: Plan dosyasi bulunamadi: $PLAN_FILE${NC}"
  exit 1
fi

echo -e "${CYAN}================================================${NC}"
echo -e "${CYAN}  SR Developer Agent${NC}"
echo -e "${CYAN}================================================${NC}"
echo ""
echo -e "${YELLOW}Plan:${NC}    $(basename "$PLAN_FILE")"
echo -e "${YELLOW}Proje:${NC}   $PROJECT_ROOT"
echo -e "${YELLOW}Mod:${NC}     acceptEdits (edit'ler otomatik, bash komutlari onay gerektirir)"
echo ""

# Plan icerigini oku
PLAN_CONTENT=$(cat "$PLAN_FILE")

# System prompt'u oku
SYSTEM_PROMPT=$(cat "$PROMPT_FILE")

# Implementation prompt'u olustur
IMPLEMENTATION_PROMPT=$(cat <<PROMPT_END
/compact

Sonra asagidaki plani adim adim uygula:

$PLAN_CONTENT

---

Her adimda:
1. Ilgili dosyayi oku
2. Degisikligi yap
3. Bir sonraki adima gec

Basla.
PROMPT_END
)

echo -e "${GREEN}Claude CLI baslatiliyor...${NC}"
echo -e "${YELLOW}Once /compact calistirilacak, sonra plan uygulanacak.${NC}"
echo ""

# Claude CLI'yi interactive modda calistir
# Prompt positional argument olarak verilir, interactive session acilir
cd "$PROJECT_ROOT"
claude \
  --permission-mode acceptEdits \
  --append-system-prompt "$SYSTEM_PROMPT" \
  "$IMPLEMENTATION_PROMPT" \
  2>&1 || {
    echo -e "${RED}Claude CLI hatasi!${NC}"
    exit 1
  }

echo ""
echo -e "${GREEN}================================================${NC}"
echo -e "${GREEN}  SR Developer Agent tamamlandi.${NC}"
echo -e "${GREEN}================================================${NC}"
