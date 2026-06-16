REST API на ASP.NET Core для аутентификации пользователей с использованием JWT-токенов и интеграцией с базой данных PostgreSQL через Docker.
Также есть Ratelimit. Реализованы следующие эндпоинты:
- /api/auth/login - ждет данные на вход { login, password } через POST-метод.
- /api/auth/profile - ждет данные на вход { Authorization Bearer <token> } через GET-метод.
