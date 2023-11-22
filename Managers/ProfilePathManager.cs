namespace EABotToTheGame.Managers
{
    //private readonly string _originalProfilePath;
    /// <summary>
    /// Класс по работе с директориями профилей для Selenium
    /// </summary>
    public class ProfilePathManager
    {      
        public ProfilePathManager()
        {
            //_originalProfilePath = originalProfilePath;
        }

        /// <summary>
        /// Метод создания копии профиля на сессию
        /// </summary>
        /// <param name="originalProfilePath"></param>
        /// <returns></returns>
        public string CreateTempProfile(string originalProfilePath)
        {
            string uniqueIdentifier = Guid.NewGuid().ToString("N"); // Генерируем уникальный идентификатор
            string tempBasePath = Path.Combine(originalProfilePath, ".."); // Указываем базовую директорию для временных профилей
            string tempPath = Path.Combine(tempBasePath, uniqueIdentifier); // Создаем уникальную директорию

            // Копируем основной профиль во временную директорию
            CopyProfile(originalProfilePath, tempPath);

            return tempPath;
        }



        private void CopyProfile(string sourcePath, string destinationPath)
        {
            try
            {
                Directory.CreateDirectory(destinationPath);

                foreach (var file in Directory.EnumerateFiles(sourcePath))
                {
                    System.IO.File.Copy(file, Path.Combine(destinationPath, Path.GetFileName(file)), true);
                }

                foreach (var dir in Directory.EnumerateDirectories(sourcePath))
                {
                    string destinationSubDir = Path.Combine(destinationPath, Path.GetFileName(dir));
                    CopyProfile(dir, destinationSubDir);
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок копирования
                Console.WriteLine($"Ошибка при копировании профиля: {ex.Message}");
            }
        }

        /// <summary>
        /// Асинхронный метод удаления директории профиля после его использования
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void DeleteDirectory(string path)
        {
            try
            {
                foreach (string directory in Directory.GetDirectories(path))
                {
                     DeleteDirectory(directory); // Рекурсивно удаляем поддиректории асинхронно
                }

                foreach (string file in Directory.GetFiles(path))
                {
                    try
                    {
                        System.IO.File.Delete(file); // Попытка удаления файла
                    }
                    catch (IOException)
                    {
                        // Файл занят другим процессом, можно добавить логику повторного удаления
                        Thread.Sleep(500); // Подождать 500 мс и повторить попытку
                        System.IO.File.Delete(file); // Повторная попытка удаления файла
                    }
                }

                Directory.Delete(path, true); // Удаляем саму директорию
            }
            catch (Exception ex)
            {
                // Обработка ошибок удаления
                Console.WriteLine($"Ошибка при удалении директории: {ex.Message}");
            }
        }

    }

}
