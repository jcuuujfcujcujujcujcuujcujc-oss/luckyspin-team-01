#!/usr/bin/env bash
# Отправляет ветки и открывает два Pull Request. Запускать после scripts/setup-github.sh.
# Рецензента укажите аргументом: bash scripts/open-pull-requests.sh <github-логин-рецензента>
set -euo pipefail
cd "$(dirname "$0")/.."

REVIEWER="${1:-}"
git push -u origin docs/review-fixes feature/iteration-1

cat > .pr-body.tmp <<'BODY'
## Что сделано

Итерация 1: перевод на .NET 10, единый `SlotEngine` и `SlotService`, проверка ставки (отрицательная ставка больше не увеличивает баланс), JSON-ошибки на русском, рабочая страница игры (добавлен скрипт), проект тестов xUnit (19 тестов), README и отчёт об итерации.

## Связанные Issues

Closes #1
Closes #2
Closes #3
Closes #4
Closes #6
Closes #7
Closes #8
Closes #12
Closes #13
Closes #14

## Проверка по Definition of Done

- [ ] Проект собирается, `dotnet test` проходит
- [ ] Для нового поведения добавлены тесты, включая негативный случай
- [ ] Критерии приёмки из Issues отмечены и проверены
- [ ] README и docs/ соответствуют коду
- [ ] В коммитах нет секретов и файлов сборки
- [ ] Ревью выполнено участником, который не является автором
BODY
ARGS=(--base main --head feature/iteration-1 --title "Итерация 1: US-01, US-02, US-03, тесты, .NET 10" --body-file .pr-body.tmp --milestone "Итерация 1")
[ -n "$REVIEWER" ] && ARGS+=(--reviewer "$REVIEWER")
gh pr create "${ARGS[@]}"

cat > .pr-body.tmp <<'BODY'
## Что сделано

Исправления по замечаниям преподавателя к лабораторным № 1–3: роли и ответственные, календарный план, порядок согласования и приёмки, граница MVP и версии 2 (NFR-01 перенесён), правила создания игрока для каждого метода API, нагрузка итераций (12 SP), процесс разработки (WORKFLOW.md), шаблон Pull Request, скрипты настройки Milestones.

## Связанные Issues

Refs #15

## Проверка по Definition of Done

- [ ] README и docs/ согласованы между собой
- [ ] Ссылки на Issues открываются
- [ ] Ревью выполнено участником, который не является автором
BODY
ARGS=(--base main --head docs/review-fixes --title "Документы: исправления по замечаниям преподавателя" --body-file .pr-body.tmp)
[ -n "$REVIEWER" ] && ARGS+=(--reviewer "$REVIEWER")
gh pr create "${ARGS[@]}"
rm -f .pr-body.tmp
echo "Готово. Попросите рецензента проверить и одобрить оба Pull Request, затем слейте их кнопкой Merge."
