#!/usr/bin/env bash
# Один запуск: создаёт репозиторий на GitHub (если его нет), отправляет main, создаёт Issues,
# Milestones и открывает два Pull Request.
# Использование:  bash scripts/publish-all.sh [github-логин-рецензента]
# Нужны GitHub CLI и вход в нужный аккаунт: gh auth login
set -euo pipefail
cd "$(dirname "$0")/.."

NAME="luckyspin-team-01"
REVIEWER="${1:-}"
OWNER=$(gh api user --jq .login)
echo "Аккаунт GitHub: $OWNER"

if gh repo view "$OWNER/$NAME" > /dev/null 2>&1; then
  echo "Репозиторий $OWNER/$NAME уже существует"
else
  gh repo create "$OWNER/$NAME" --public --description "Учебный проект: веб-API слот-игры на ASP.NET Core"
fi

if git remote get-url origin > /dev/null 2>&1; then
  git remote set-url origin "https://github.com/$OWNER/$NAME.git"
else
  git remote add origin "https://github.com/$OWNER/$NAME.git"
fi

git push -u origin main

COUNT=$(gh issue list --repo "$OWNER/$NAME" --state all --limit 200 --json number --jq 'length')
if [ "$COUNT" -eq 0 ]; then
  bash scripts/create-issues.sh
else
  echo "В репозитории уже есть Issues ($COUNT), шаг создания Issues пропущен"
fi

bash scripts/setup-github.sh
bash scripts/open-pull-requests.sh $REVIEWER
