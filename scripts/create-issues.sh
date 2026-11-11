#!/usr/bin/env bash
# Создаёт метки и Issues #1..#14 в текущем репозитории. Нужны GitHub CLI и вход: gh auth login
# Запускать в НОВОМ репозитории без Issues: номера #1..#14 должны совпасть со ссылками в docs/.
set -euo pipefail
cd "$(dirname "$0")/.."

gh label create requirement --color 1d76db --description "Требование" --force
gh label create feature --color 0e8a16 --description "Функциональность" --force
gh label create quality --color fbca04 --description "Нефункциональное требование" --force
gh label create user-story --color 5319e7 --description "Пользовательская история" --force
gh label create technical --color 6a737d --description "Техническая задача" --force

for f in docs/issues/0[1-9]-*.md docs/issues/1[0-4]-*.md; do
  title=$(sed -n '1s/<!-- title: \(.*\) -->/\1/p' "$f" | tr -d '\r')
  labels=$(sed -n '2s/<!-- labels: \(.*\) -->/\1/p' "$f" | tr -d '\r')
  sed 1,5d "$f" > .issue-body.tmp
  gh issue create --title "$title" --label "$labels" --body-file .issue-body.tmp
done
rm -f .issue-body.tmp
echo "Готово: создано 14 Issues"
