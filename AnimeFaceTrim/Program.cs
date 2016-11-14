using System;
using OpenCvSharp;
using System.IO;
using Mono.Options;


namespace AnimeFaceTrim
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				string filepath = null;
				var p = new OptionSet()
					.Add("f|file=", "File Path", v => filepath = v);

				p.Parse(args);

				if (filepath == null)
				{
					Console.Error.WriteLine("Usage:AnimeFaceTrim [OPTIONS]");
					Console.Error.WriteLine();
					p.WriteOptionDescriptions(Console.Error);
					return;
				}

				Mat image = Cv2.ImRead(filepath);

				string xml = @"xml\lbpcascade_animeface.xml";
				Mat[] detectFaceImage = DetectFaceInImage(image, Path.Combine(Directory.GetCurrentDirectory(), xml));
				for(int i = 0; i < detectFaceImage.Length; i++)
				{
					string outfile = Path.Combine(Path.GetDirectoryName(filepath), Path.GetFileNameWithoutExtension(filepath) + "_trim" + (i+1) + Path.GetExtension(filepath));
					detectFaceImage[i].SaveImage(outfile);
				}

				Console.WriteLine("Detect Face Success. ");
			}
			catch(Exception)
			{
				Console.WriteLine("Detect Face Fail");
			}
			finally
			{

			}
		}


		static Mat[] DetectFaceInImage(Mat image, string cascade_file)
		{
			CascadeClassifier cascade = new CascadeClassifier();
			cascade.Load(cascade_file);
			Rect[] faces = cascade.DetectMultiScale(image, 1.1, 3, 0, new Size(20, 20));
			Mat[] images = new Mat[faces.Length];

			for (int facesPointer = 0; facesPointer < faces.Length; facesPointer++)
			{

				//　横位置取得
				int nx = faces[facesPointer].X;

				//　検出幅を1/3にして、横方向切り出し位置の基準とする。
				int wbase = faces[facesPointer].Width;

				//　縦位置取得
				int ny = faces[facesPointer].Y;

				//　縦方向切り出し位置の基準とする。
				int hbase = faces[facesPointer].Height;

				//　切り出し幅を幅基準の５倍とする。
				int newWidth = (int)(wbase);

				//　切り出し高さ幅基準
				int newHeight = (int)(wbase);

				//　切り出し位置を作成
				int colEnd = nx + newWidth;
				int rowEnd = ny + newHeight;

				// 計算位置が元写真より大きい場合は、元写真の大きさに修正
				if (colEnd > image.Width)
				{
					colEnd = image.Width;
				}
				if (rowEnd > image.Height)
				{
					rowEnd = image.Height;
				}

				//　元写真から検知部分を切り抜き
				images[facesPointer] = image[ny, rowEnd, nx, colEnd];
			}

			return images;

		}
	}
}
