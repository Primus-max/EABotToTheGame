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
        _driver = driver ?? throw new ArgumentNullException(nameof(driver));
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
        var screenshotPath = Path.Combine(outputDirectory, $"{fileName}_wrong.png");

        if (fullScreenshot)
        {
            // Если нужен полный скриншот, сохраняем его и возвращаем путь
            screenshot.SaveAsFile(screenshotPath, ScreenshotImageFormat.Png);
            return screenshotPath;
        }
        else
        {
            // Если нужен обрезанный скриншот, обрезаем и сохраняем
            int x = 100;
            int y = 100;
            int width = 500;
            int height = 500;

            using (var image = new MagickImage(screenshot.AsByteArray))
            {
                image.Crop(new MagickGeometry(x, y, width, height));

                // Создаем имя для обрезанного скриншота
                var croppedFileName = $"{fileName}.png";

                // Сохраняем обрезанный скриншот и возвращаем путь
                var croppedScreenshotPath = Path.Combine(outputDirectory, croppedFileName);
                image.Write(croppedScreenshotPath);

                return croppedScreenshotPath;
            }
        }
    }

}

