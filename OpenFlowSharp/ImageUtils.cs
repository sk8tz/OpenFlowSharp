using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;

namespace OpenFlowSharp
{
	public static class ImageUtils
	{	
		public static UIImage AddImageReflection (UIImage image, float reflectionFraction)
		{
			int reflectionHeight = (int) (image.Size.Height * reflectionFraction);
			
			// Create a 2 bit CGImage containing a gradient that will be used for masking the
			// main view content to create the 'fade' of the reflection.  The CGImageCreateWithMask
			// function will stretch the bitmap image as required, so we can create a 1 pixel wide gradient
			

			// gradient is always black and white and the mask must be in the gray colorspace
			var colorSpace = CGColorSpace.CreateDeviceGray ();
			
			// Creat the bitmap context
			var gradientBitmapContext = new CGBitmapContext (IntPtr.Zero, 1, reflectionHeight, 8, 0, colorSpace, CGImageAlphaInfo.None);
			
			// define the start and end grayscale values (with the alpha, even though
			// our bitmap context doesn't support alpha the gradien requires it)
			float [] colors = { 0, 1, 1, 1 };
			
			// Create the CGGradient and then release the gray color space
			var grayScaleGradient = new CGGradient (colorSpace, colors, null);
			colorSpace.Dispose ();
			
			// create the start and end points for the gradient vector (straight down)
			var gradientStartPoint = new PointF (0, reflectionHeight);
			var gradientEndPoint = PointF.Empty;
			
			// draw the gradient into the gray bitmap context
			gradientBitmapContext.DrawLinearGradient (grayScaleGradient, gradientStartPoint, 
			                                          gradientEndPoint, CGGradientDrawingOptions.DrawsAfterEndLocation);
			grayScaleGradient.Dispose ();

			// Add a black fill with 50% opactiy
			gradientBitmapContext.SetGrayFillColor (0, 0.5f);
			gradientBitmapContext.FillRect (new RectangleF (0, 0, 1, reflectionHeight));
			                                
            // conver the context into a CGImage and release the context
			var gradientImageMask = gradientBitmapContext.ToImage ();
			gradientBitmapContext.Dispose ();
			
			// create an image by masking the bitmap of the mainView content with the gradient view
			// then release the pre-masked content bitmap and the gradient bitmap
			var reflectionImage = image.CGImage.WithMask (gradientImageMask);
			gradientImageMask.Dispose ();
			
			var size = new SizeF (image.Size.Width, image.Size.Height + reflectionHeight);
			
			UIGraphics.BeginImageContext (size);
			image.Draw (PointF.Empty);
			var context = UIGraphics.GetCurrentContext ();
			context.DrawImage (new RectangleF (0, image.Size.Height, image.Size.Width, reflectionHeight), reflectionImage);
			
			var result = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			reflectionImage.Dispose ();
			
			return result;
		}
	}
}
