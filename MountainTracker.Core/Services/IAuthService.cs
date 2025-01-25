using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.Services
{
    /// <summary>
    /// Интерфейс для аутентификации/авторизации
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="nickname">Ник/имя</param>
        /// <returns>Идентификатор созданного пользователя</returns>
        Task<Guid> RegisterUserAsync(string login, string password, string nickname);

        /// <summary>
        /// Проверка логина/пароля, выдача токенов
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>JWT или другой объект авторизации</returns>
        Task<string> LoginAsync(string login, string password);

        /// <summary>
        /// Валидация токена / получение идентификатора пользователя 
        /// (если нужно в Core)
        /// </summary>
        /// <param name="token">JWT-токен</param>
        /// <returns>Id пользователя или null</returns>
        Task<Guid?> ValidateTokenAsync(string token);
    }
}

