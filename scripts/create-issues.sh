#!/usr/bin/env bash
# Создаёт метки и Issues в текущем репозитории. Нужны установленный GitHub CLI и вход: gh auth login
# Запускать в НОВОМ репозитории без Issues: номера #1..#14 должны совпасть со ссылками в docs/.
set -euo pipefail
cd "$(dirname "$0")/.."
gh label create requirement --color 1d76db --description "Требование" --force
gh label create feature --color 0e8a16 --description "Функциональность" --force
gh label create quality --color fbca04 --description "Нефункциональное требование" --force
gh label create user-story --color 5319e7 --description "Пользовательская история" --force
gh label create technical --color 6a737d --description "Техническая задача" --force

gh issue create --title "FR-01: Игрок получает баланс по идентификатору" --label "requirement,feature" --body-file <(sed 1,5d "docs/issues/01-fr-01.md")  # #1
gh issue create --title "FR-02: Игрок совершает ставку (spin)" --label "requirement,feature" --body-file <(sed 1,5d "docs/issues/02-fr-02.md")  # #2
gh issue create --title "FR-03: Система начисляет выигрыш по правилам игры" --label "requirement,feature" --body-file <(sed 1,5d "docs/issues/03-fr-03.md")  # #3
gh issue create --title "FR-04: Система отклоняет некорректные ставки" --label "requirement,feature" --body-file <(sed 1,5d "docs/issues/04-fr-04.md")  # #4
gh issue create --title "FR-05: Игрок просматривает историю своих игр" --label "requirement,feature" --body-file <(sed 1,5d "docs/issues/05-fr-05.md")  # #5
gh issue create --title "US-01: Как игрок, я хочу видеть свой баланс, чтобы понимать, сколько могу поставить" --label "user-story,feature" --body-file <(sed 1,5d "docs/issues/06-us-01.md")  # #6
gh issue create --title "US-02: Как игрок, я хочу сделать ставку и увидеть результат вращения, чтобы играть в слот" --label "user-story,feature" --body-file <(sed 1,5d "docs/issues/07-us-02.md")  # #7
gh issue create --title "US-03: Как игрок, я хочу получать понятную ошибку при неверной ставке, чтобы исправить её и не потерять кредиты" --label "user-story,feature" --body-file <(sed 1,5d "docs/issues/08-us-03.md")  # #8
gh issue create --title "US-04: Как игрок, я хочу видеть историю своих вращений, чтобы контролировать свою игру" --label "user-story,feature" --body-file <(sed 1,5d "docs/issues/09-us-04.md")  # #9
gh issue create --title "US-05: Как оператор, я хочу видеть список игроков и их балансы, чтобы контролировать состояние системы" --label "user-story,feature" --body-file <(sed 1,5d "docs/issues/10-us-05.md")  # #10
gh issue create --title "US-06: Как оператор, я хочу блокировать игрока, чтобы пресекать нарушения правил" --label "user-story,feature" --body-file <(sed 1,5d "docs/issues/11-us-06.md")  # #11
gh issue create --title "TECH-01: Убрать шаблонный код и объединить логику слота в SlotEngine" --label "technical" --body-file <(sed 1,5d "docs/issues/12-tech-01.md")  # #12
gh issue create --title "TECH-02: Добавить проект автоматических тестов (xUnit)" --label "technical" --body-file <(sed 1,5d "docs/issues/13-tech-02.md")  # #13
gh issue create --title "TECH-03: Обновить целевую платформу с .NET Core 3.1" --label "technical" --body-file <(sed 1,5d "docs/issues/14-tech-03.md")  # #14
echo "Готово: создано 14 Issues"
