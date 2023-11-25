using ImageMagick;

public class ScreenshotService
{
    private readonly IWebDriver _driver;

    /// <summary>
    /// Класс для снятия скриншота
    /// </summary>
    /// <param name="driver"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ScreenshotService(IWebDriver driver)
    {
        _driver = driver ;
    }

    /// <summary>
    /// Делаю скриншот
    /// </summary>
    /// <returns></returns>
    public string CaptureAndCropScreenshot(bool fullScreenshot = false)
    {
        string outputDirectory = "screenshots"; // Имя директории
        var fileName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}"; // Имя файла

        // Создаем директорию, если ее нет
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // Создаем скриншот
        var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();

        // Создаем имя файла и путь для сохранения
        var screenshotPath = Path.Combine(outputDirectory, $"{fileName}_full.png");

        // Сохраняем полный скриншот в файл
        screenshot.SaveAsFile(screenshotPath, ScreenshotImageFormat.Png);

        if (fullScreenshot)
        {
            // Если нужен полный скриншот, возвращаем путь
            return screenshotPath;
        }
        else
        {
            using (var image = new MagickImage(screenshotPath))
            {
                // Задаем размеры обрезки и отступы (в данном примере)
                int cropWidth = 400;
                int cropHeight = 80;
                int topOffset = 60;

                // Вычисляем координаты правого верхнего угла
                int rightX = image.Width - cropWidth;
                int topY = topOffset;

                // Обрезаем изображение
                image.Crop(new MagickGeometry(rightX, topY, cropWidth, cropHeight));

                // Создаем имя для обрезанного изображения
                var croppedFileName = $"cropped_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                var croppedImagePath = Path.Combine(outputDirectory, croppedFileName);

                // Сохраняем обрезанное изображение
                image.Write(croppedImagePath);

                // Возвращаем путь к обрезанному изображению
                return croppedImagePath;
            }
        }
    }

}

