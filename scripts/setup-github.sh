#!/usr/bin/env bash
# Настраивает GitHub для проекта: метки, Milestones, Issue #15 (NFR-01), привязка Issues к Milestones
# и назначение исполнителя. Нужны GitHub CLI и вход: gh auth login. Запускать один раз.
set -euo pipefail
cd "$(dirname "$0")/.."

REPO=$(gh repo view --json nameWithOwner -q .nameWithOwner)
echo "Репозиторий: $REPO"

gh label create quality --color fbca04 --description "Нефункциональное требование" --force

create_milestone() {
  local title="$1" desc="$2" due="$3"
  if gh api "repos/$REPO/milestones?state=all" --jq '.[].title' | grep -Fxq "$title"; then
    echo "Milestone уже есть: $title"
  else
    gh api "repos/$REPO/milestones" -f title="$title" -f description="$desc" -f due_on="$due" > /dev/null
    echo "Создан Milestone: $title"
  fi
}

create_milestone "Итерация 1" "US-01, US-02, US-03, TECH-02, FR-01..FR-04" "2026-11-20T23:59:59Z"
create_milestone "Итерация 2" "TECH-01, TECH-03, US-04, FR-05; приёмка MVP" "2026-12-04T23:59:59Z"
create_milestone "Итерация 3" "Версия 2: NFR-01, US-05, US-06" "2026-12-18T23:59:59Z"

# Issue #15 (NFR-01) создаётся, если Issues ещё меньше 15
COUNT=$(gh issue list --state all --limit 200 --json number --jq 'length')
if [ "$COUNT" -lt 15 ]; then
  sed 1,5d docs/issues/15-nfr-01.md > .issue-body.tmp
  gh issue create --title "NFR-01: Хеши паролей и роли доступа (версия 2)" --label "requirement,quality" --body-file .issue-body.tmp
  rm -f .issue-body.tmp
fi

assign() {
  local milestone="$1"; shift
  for n in "$@"; do
    gh issue edit "$n" --milestone "$milestone" --add-assignee "@me"
  done
}

assign "Итерация 1" 1 2 3 4 6 7 8 13
assign "Итерация 2" 5 9 12 14
assign "Итерация 3" 10 11 15

echo "Готово: Milestones созданы, Issues привязаны и назначены."
