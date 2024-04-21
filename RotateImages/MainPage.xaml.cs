using SkiaSharp;
using System.Diagnostics;

namespace RotateImages;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnRotate(object sender, EventArgs e)
	{
		//reset labels
		time.Text = "Elapsed time: ";
		thread.Text = "Threads: ";

        /*get current directory does not work with maui and I could 
        not find a way past it unless I hard coded my exact file path*/
        //HARD CODED FILE PATH, CHANGE VARIABLE "path" TO WHERE YOUR CAT PHOTOS ARE
        string path = @"C:\Users\cache\Downloads\pictures";

        var images = Directory.GetFiles(path);
		//set each picture to new path picture...not great code
		cat1.Source = images[0];
		cat2.Source = images[1];
		cat3.Source = images[2];
		cat4.Source = images[3];


        //var newPath = path + @"\RotatedImages";

        RotateAndSave(images, path);
    }

	private void RotateAndSave(string[] images, string newPath)
	{
		Stopwatch sw = Stopwatch.StartNew();
		List<int> list = new List<int>();
		Parallel.ForEach(images, image =>
		{
			var file = Path.GetFileName(image);
			using (var bitmap = SKBitmap.Decode(image))
			{
				var rotated = new SKBitmap(bitmap.Height, bitmap.Width);

				using (var surface = new SKCanvas(rotated))
				{
					surface.Translate(rotated.Width, 0);
					surface.RotateDegrees(90);
					surface.DrawBitmap(bitmap, 0, 0);
				}
                var save = rotated.Encode(SKEncodedImageFormat.Jpeg, 100);
                using (var stream = File.OpenWrite(Path.Combine(newPath, $"{image}")))
                {
                    // save the data to a stream
                    save.SaveTo(stream);
                }
            }
			list.Add(Thread.CurrentThread.ManagedThreadId); // get threads in list, can't access UI from thread

        });
		time.Text += sw.ElapsedMilliseconds.ToString() + "ms";
		foreach (int id in list)
		{
			thread.Text += id + "   ";
		}
		SemanticScreenReader.Announce(thread.Text);
        SemanticScreenReader.Announce(time.Text);
        sw.Stop();
	}

}

