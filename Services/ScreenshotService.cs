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
    public string CaptureAndCropScreenshot()
    {
        string outputDirectory = "screenshots"; // Имя директории
        var fileName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}"; // Имя файлы

        int x = 100;
        int y = 100;
        int width = 500;
        int height = 500;

        // Создаем директорию, если ее нет
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // Создаем полный скриншот
        var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();

        // Сохраняем полный скриншот
        var fullScreenshotPath = Path.Combine(outputDirectory, $"{fileName}_full.png");
        screenshot.SaveAsFile(fullScreenshotPath, ScreenshotImageFormat.Png);

        // Загружаем полный скриншот и обрезаем его до нужной области
        using (var image = new MagickImage(fullScreenshotPath))
        {
            image.Crop(new MagickGeometry(x, y, width, height));

            // Создаем имя для обрезанного скриншота
            var croppedFileName = $"{fileName}.png";

            // Сохраняем обрезанный скриншот
            var croppedScreenshotPath = Path.Combine(outputDirectory, croppedFileName);
            image.Write(croppedScreenshotPath);

            return croppedScreenshotPath;
        }
    }
}

