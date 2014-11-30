using ASDAGeorgeApp.Interface;
using ASDAGeorgeApp.Models;
using ASDAGeorgeApp.ViewModels;
using ASDAGeorgeApp.Views;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Variables
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensorChooser _sensorChooser;
        public KinectSensorChooser sensorChooser
        {
            get { return _sensorChooser; }
            set
            {
                if (_sensorChooser != value)
                {
                    _sensorChooser = value;
                    NotifyPropertyChanged();
                }
            }
        }

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
        private WriteableBitmap _colorBitmap;
        public WriteableBitmap colorBitmap
        {
            get { return _colorBitmap; }
            set
            {
                if (_colorBitmap != value)
                {
                    _colorBitmap = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup _drawingGroup;
        public DrawingGroup drawingGroup
        {
            get { return _drawingGroup; }
            set
            {
                if (_drawingGroup != value)
                {
                    _drawingGroup = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage _imageSource;
        public DrawingImage imageSource
        {
            get { return _imageSource; }
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] _colorPixels;
        public byte[] colorPixels
        {
            get { return _colorPixels; }
            set
            {
                if (_colorPixels != value)
                {
                    _colorPixels = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region ClothesRegion
        private Item _Product;
        public Item Product
        {
            get { return _Product; }
            set
            {
                if (_Product != value)
                {
                    _Product = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _ProductTextSpacing;
        public int ProductTextSpacing
        {
            get { return _ProductTextSpacing; }
            set
            {
                if(_ProductTextSpacing != value)
                {
                    _ProductTextSpacing = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        public MainWindow()
        {
            /* HACK : This keeps the splashscreen from disappearing by adding a second to the load time for the Initialisation */
            // Thread.Sleep(5000);
            InitializeComponent();
            Collector.LoadInformation();

            ProductTextSpacing = 20;

            Switcher.pageSwitcher = this;
            Switcher.Switch(new LandingPage());

            /* initialize the sensor chooser and UI */
            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooser.KinectChanged += this.SensorChooserOnKinectChanged;
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();

            // Bind the sensor chooser's current sensor to the KinectRegion
            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
        }

        #region Clothes Function
        /// <summary>
        /// Sets the location of the Element given to the Point given
        /// </summary>
        /// <param name="element">The element to set the location of</param>
        /// <param name="point">The location to be set to</param>
        private void SetImagePosition(FrameworkElement element, ColorImagePoint point)
        {
            InkCanvas.SetLeft(element, point.X - element.Width / 2);
            InkCanvas.SetTop(element, point.Y - element.Width / 2);
            MessageBox.Show(Canvas.GetLeft(element).ToString());
            MessageBox.Show(Canvas.GetTop(element).ToString());
        }

        /// <summary>
        /// Draws the product out if available
        /// </summary>
        /// <param name="skel">The skeleton to draw from</param>
        private void DrawProduct(Skeleton skel, DrawingContext dc)
        {
            if (Product != null)
            {
                /* Display product */
                try
                {
                    DisplayProductOnUser(skel, dc);
                    DisplayInformationNextToUser(skel, dc);
                }
                catch (Exception e)
                {
                    DisplayErrorOnUser(skel, dc, e);
                }
            }

            return;
        }

        /// <summary>
        /// Display the Title and Price next to the user
        /// </summary>
        /// <param name="skel">The skeleton to put the price next to</param>
        private void DisplayInformationNextToUser(Skeleton skel, DrawingContext dc)
        {
            if (skel == null)
                throw new ArgumentNullException("The skeleton was null upon trying to place information next to the user");

            /* Find which shoulder is more to the right on the screen */
            ColorImagePoint pointToUse = GetPointToUse(skel);

            /* Put product text onto screen next to right shoulder */
            // do shit TODO
        }

        private double GetDepthPointToUse(Skeleton skel)
        {
            /* Get the points of the right and left shoulders */
            CoordinateMapper coordMapper = new CoordinateMapper(sensorChooser.Kinect);
            DepthImagePoint rightShoulder = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.ShoulderRight].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint leftShoulder = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.ShoulderLeft].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint rightElbow = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.ElbowRight].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint leftElbow = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.ElbowLeft].Position, DepthImageFormat.Resolution640x480Fps30);

            if (rightShoulder == null || leftShoulder == null)
                throw new ArgumentNullException("Could not find one or both shoulders");
            
            if (rightShoulder.X > leftShoulder.X)
            {
                return (rightShoulder.X - leftShoulder.X) * 1.5;
            }
            else
            {
                return (leftShoulder.X - rightShoulder.X) * 1.5;
            }
        }

        private ColorImagePoint GetPointToUse(Skeleton skel)
        {
            /* Get the points of the right and left shoulders */
            CoordinateMapper coordMapper = new CoordinateMapper(sensorChooser.Kinect);
            ColorImagePoint rightShoulder = coordMapper.MapSkeletonPointToColorPoint(skel.Joints[JointType.ShoulderRight].Position, ColorImageFormat.RgbResolution640x480Fps30);
            ColorImagePoint leftShoulder = coordMapper.MapSkeletonPointToColorPoint(skel.Joints[JointType.ShoulderLeft].Position, ColorImageFormat.RgbResolution640x480Fps30);

            if (rightShoulder == null || leftShoulder == null)
                throw new ArgumentNullException("Could not find one or both shoulders");

            if (rightShoulder.X > leftShoulder.X)
                return rightShoulder;
            else
                return leftShoulder;
        }

        private void DisplayErrorOnUser(Skeleton skel, DrawingContext dc, Exception e)
        {
            if (skel == null)
                throw new ArgumentNullException("The skeleton was null upon trying to place information next to the user");

            ColorImagePoint pointToUse = GetPointToUse(skel);

            PlaceTextOnUser(pointToUse, dc, e.Message);
        }

        private void PlaceTextOnUser(ColorImagePoint pointToUse, DrawingContext dc, string p)
        {
            pointToUse.X += ProductTextSpacing;

            Point point = new Point(pointToUse.X, pointToUse.Y);

            dc.DrawText(new FormattedText(p, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Segoe"), 24, Brushes.White), point);
        }

        private void PlaceTextOnUser(ColorImagePoint pointToUse, DrawingContext dc)
        {
            double xCoord = pointToUse.X;
            xCoord = xCoord + ProductTextSpacing;

            TextBlock floating = new TextBlock();
            floating.Text = Product.Title + "£" + Product.Price;
            SetImagePosition(floating, pointToUse);
        }

        private void DisplayProductOnUser(Skeleton skel, DrawingContext dc)
        {
            if (skel == null)
                throw new ArgumentNullException("The skeleton was null upon trying to place information next to the user");

            /* Get the coords of the body */
            CoordinateMapper coordMapper = new CoordinateMapper(sensorChooser.Kinect);
            DepthImagePoint centerShoulder = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.ShoulderCenter].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint rightShoulder = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.ShoulderRight].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint leftShoulder = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.ShoulderLeft].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint rightElbow = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.ElbowRight].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint leftElbow = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.ElbowLeft].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint rightHip = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.HipRight].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint leftHip = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.HipLeft].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint rightKnee = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.KneeRight].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint leftKnee = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.KneeLeft].Position, DepthImageFormat.Resolution640x480Fps30);

            double width = GetDepthPointToUse(skel);
            double x = centerShoulder.X - (width / 2);
            double y = centerShoulder.Y + ((leftShoulder.Y - centerShoulder.Y) / 2);
            double height = (rightHip.Y - y) * 1.3;

            ImageSource image = new BitmapImage(new Uri(@"D:\Documents\Dropbox\University\Third Year\Advanced Human Computer Interaction\Coursework 2\ASDAGeorge\ASDAGeorgeApp\image\marvel_tee_psd.png"));

            dc.DrawImage(image, new Rect(x, y, width, height));
        }

        #endregion

        #region CodeBehind Functions

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }
            
            using (DrawingContext dc = this.drawingGroup.Open())
            {
                /* Draw a transparent background to set the render size */
                dc.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        // RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);

                            DrawProduct(skel, dc);
                            break;
                        }
                    }
                }

                /* prevent drawing outside of our render area */
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            /* Convert point to depth space.
               We are not using depth directly, but we do want the points in our 640x480 output resolution. */
            try
            {
                DepthImagePoint depthPoint = this.sensorChooser.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
                return new Point(depthPoint.X, depthPoint.Y);
            }
            catch(Exception e)
            { return new Point(0, 0); }
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            /* If we can't find either of these joints, exit */
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            /* Don't draw if both points are inferred */
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            /* We assume all drawn bones are inferred unless BOTH joints are tracked */
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }

            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            /* Render Torso */
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            /* Left Arm */
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            /* Right Arm */
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            /* Left Leg */
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            /* Right Leg */
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);

            /* Render Joints */
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    /* Copy the pixel data from the image to a temporary array */
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    /* Write the pixel data into our bitmap */
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
                    /* Disable Streams */
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.ColorStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();

                    /* Remove an event handler to be called whenever there is new color frame data */
                    args.OldSensor.SkeletonFrameReady -= this.SensorSkeletonFrameReady;

                    /* Null space to put the pixels we'll receive */
                    this.colorPixels = null;

                    /* This is the bitmap we'll display on-screen, nulled */
                    this.colorBitmap = null;

                    /* Remove an event handler to be called whenever there is new color frame data */
                    args.OldSensor.ColorFrameReady -= this.SensorColorFrameReady;

                    /* Null the drawing group we'll use for drawing */
                    this.drawingGroup = null;
                }
                catch (InvalidOperationException)
                {
                    /* KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                       E.g.: sensor might be abruptly unplugged. */
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    /* Turn on the streams to receive frames */
                    args.NewSensor.SkeletonStream.Enable();
                    args.NewSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

                    /* Add an event handler to be called whenever there is new color frame data */
                    args.NewSensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                    /* Allocate space to put the pixels we'll receive */
                    this.colorPixels = new byte[args.NewSensor.ColorStream.FramePixelDataLength];

                    /* This is the bitmap we'll display on-screen */
                    this.colorBitmap = new WriteableBitmap(args.NewSensor.ColorStream.FrameWidth, this.sensorChooser.Kinect.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                    /* Set the image we display to point to the bitmap where we'll put the image data */
                    this.KinectImage.Source = this.colorBitmap;

                    /* Add an event handler to be called whenever there is new color frame data */
                    args.NewSensor.ColorFrameReady += this.SensorColorFrameReady;

                    /* Create the drawing group we'll use for drawing */
                    this.drawingGroup = new DrawingGroup();

                    /* Create an image source that we can use in our image control */
                    this.imageSource = new DrawingImage(this.drawingGroup);

                    /* Display the drawing using our image control */
                    Image.Source = this.imageSource;

                }
                catch (InvalidOperationException)
                {
                    /* KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                       E.g.: sensor might be abruptly unplugged. */
                }
            }
        }

#endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName = null)
        {
            if (propertyName == null)
            {
                StackTrace trace = new StackTrace();
                StackFrame frame = trace.GetFrame(1);
                MethodBase method = frame.GetMethod();
                propertyName = method.Name.Replace("set_", "");
            }

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Window Functions
        private void ExitApplication_Click(object sender, RoutedEventArgs e)
        {
            this.Window_Closing(sender, new CancelEventArgs(true));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (this.sensorChooser != null)
            {
                this.sensorChooser.Stop();
                this.sensorChooser = null;
            }
            Application.Current.Shutdown();
        }

        public void Navigate(UserControl nextPage)
        {
            if (nextPage.GetType() == typeof(ProductPage))
            {
                Product = ((ProductPage)nextPage).Product;
            }
            else if (nextPage.GetType() == typeof(LandingPage))
            {
                Product = ((LandingPage)nextPage).CurrentProduct;
            }
            else
                Product = null;

            this.kinectRegion.Content = nextPage;
        }

        public void Navigate(UserControl nextPage, object state)
        {
            this.kinectRegion.Content = nextPage;
            ISwitchable s = nextPage as ISwitchable;

            if (s != null)
                s.UtiliseState(state);
            else
                throw new ArgumentException("NextPage is not ISwitchable! " + nextPage.Name.ToString());
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Window_Closing(sender, new CancelEventArgs(true));
        }

        #endregion
    }
}
