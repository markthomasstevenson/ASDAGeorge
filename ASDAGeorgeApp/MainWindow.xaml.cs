using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ASDAGeorgeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensorChooser sensorChooser;

        /// <summary>
        /// Format we will use for the depth stream
        /// </summary>
        private const DepthImageFormat DepthFormat = DepthImageFormat.Resolution320x240Fps30;

        /// <summary>
        /// Format we will use for the color stream
        /// </summary>
        private const ColorImageFormat ColorFormat = ColorImageFormat.RgbResolution640x480Fps30;

        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;

        public MainWindow()
        {
            /* HACK : This keeps the splashscreen from disappearing by adding a second to the load time for the Initialisation */
            // Thread.Sleep(5000);
            InitializeComponent();

            // initialize the sensor chooser and UI
            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.KinectChanged += this.SensorChooserOnKinectChanged;
            this.sensorChooser.Start();

            if (null != this.sensorChooser.Kinect)
            {
                // Turn on the color stream to receive color frames
                this.sensorChooser.Kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                // Allocate space to put the pixels we'll receive
                this.colorPixels = new byte[this.sensorChooser.Kinect.ColorStream.FramePixelDataLength];

                // This is the bitmap we'll display on-screen
                this.colorBitmap = new WriteableBitmap(this.sensorChooser.Kinect.ColorStream.FrameWidth, this.sensorChooser.Kinect.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                // Set the image we display to point to the bitmap where we'll put the image data
                this.KinectImage.Source = this.colorBitmap;

                // Add an event handler to be called whenever there is new color frame data
                this.sensorChooser.Kinect.ColorFrameReady += this.SensorColorFrameReady;

                // Start the sensor!
                try
                {
                    this.sensorChooser.Kinect.Start();
                }
                catch (IOException)
                {
                    this.sensorChooser.Kinect.Dispose();
                }
            }
        }

        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }
        }

        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.ColorStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthFormat);
                    args.NewSensor.ColorStream.Enable(ColorFormat);
                    args.NewSensor.SkeletonStream.Enable();

                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }
        }

        private void ExitApplication_Click(object sender, RoutedEventArgs e)
        {
            this.Window_Closing(sender, new CancelEventArgs(true));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.sensorChooser.Stop();
            this.sensorChooser = null;
            Application.Current.Shutdown();
        }

    }
}
