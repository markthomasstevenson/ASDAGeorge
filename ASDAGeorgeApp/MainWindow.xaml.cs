using ASDAGeorgeApp.Interface;
using ASDAGeorgeApp.Models;
using ASDAGeorgeApp.ViewModels;
using ASDAGeorgeApp.Views;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private int _ProductTextSpacing = 20;
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

        private double _ClothingHeight = 0;
        public double ClothingHeight
        {
            get { return _ClothingHeight; }
            set
            {
                if (_ClothingHeight != value)
                {
                    _ClothingHeight = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region SpeechRegion
        private SpeechRecognitionEngine speechEngine;

        private Grammar ActivateGrammar;
        private Grammar NavigateGrammar;
        private Grammar SearchCatGrammar;
        private Grammar SearchSubCatGrammar;
        private Grammar WishListGrammar;

        private bool _IsListening = true;
        public bool IsListening
        {
            get { return _IsListening; }
            set
            {
                if (_IsListening != value)
                {
                    _IsListening = value;
                    NotifyPropertyChanged();
                    Collector.IsListening = _IsListening;
                }
                EnableSpeechGrammars();
            }
        }

        private bool _IsProduct = false;
        public bool IsProduct
        {
            get { return _IsProduct; }
            set
            {
                if (_IsProduct != value)
                {
                    _IsProduct = value;
                    NotifyPropertyChanged();
                }
                EnableSpeechGrammars();
            }
        }

        private bool _IsSearch = false;
        public bool IsSearch
        {
            get { return _IsSearch; }
            set
            {
                if (_IsSearch != value)
                {
                    _IsSearch = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Collector.LoadInformation();
            
            /* Load the speech recognition engine */
            RecognizerInfo ri = Collector.GetKinectRecognizer();
            if (ri != null)
                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

            CreateSpeechGrammars();

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

        #region SpeechFunctionRegion

        private void CreateSpeechGrammars()
        {
            // Create SemanticResultValue objects that contain activator possibilities
            SemanticResultValue okayGeorge = new SemanticResultValue("Okay George", "Start Listening");
            SemanticResultValue georgeListen = new SemanticResultValue("George Listen", "Start Listening");
            SemanticResultValue stopListening = new SemanticResultValue("Stop Listening", "Stop Listening");

            // Create Activator 'choices'
            Choices choicesActivator = new Choices();
            choicesActivator.Add(new Choices(new GrammarBuilder[] { okayGeorge, georgeListen, stopListening }));

            // Build the phrase and add 'choices'
            GrammarBuilder grammarActivator = new GrammarBuilder();
            grammarActivator.Append(new SemanticResultKey("activator", choicesActivator));

            // Build a Grammar object from the GrammarBuilder.
            ActivateGrammar = new Grammar(grammarActivator);

            // Create SemanticResultValue objects that contain Navigation possibilities
            SemanticResultValue go = new SemanticResultValue("Go", "Navigate");
            SemanticResultValue goTo = new SemanticResultValue("Go to", "Navigate");
            SemanticResultValue navigateTo = new SemanticResultValue("Navigate to", "Navigate");
            SemanticResultValue menuShop = new SemanticResultValue("Shop", "Shop");
            SemanticResultValue menuAccount = new SemanticResultValue("Account", "Account");
            SemanticResultValue menuWish_list = new SemanticResultValue("Wish list", "Wishlist");
            SemanticResultValue menuWishlist = new SemanticResultValue("Wishlist", "Wishlist");
            SemanticResultValue menuSearch = new SemanticResultValue("Search", "Search");
            SemanticResultValue menuHome = new SemanticResultValue("Home", "Home");

            // Create Navigator 'Choices'
            Choices choicesNavigatorActivate = new Choices();
            choicesNavigatorActivate.Add(new Choices(new GrammarBuilder[] { go, goTo, navigateTo }));

            Choices choicesNavigation = new Choices();
            choicesNavigation.Add(new Choices(new GrammarBuilder[] { menuShop, menuAccount, menuWish_list, menuWishlist, menuSearch, menuHome }));

            // build the phrasing
            GrammarBuilder grammarNavigation = new GrammarBuilder();
            grammarNavigation.Append(new SemanticResultKey("activator", choicesNavigatorActivate));
            grammarNavigation.Append(new SemanticResultKey("where", choicesNavigation));

            NavigateGrammar = new Grammar(grammarNavigation);

            // Create SemanticResultValue objects that contain search possibilities
            SemanticResultValue search = new SemanticResultValue("Search", "Search");
            SemanticResultValue searchFor = new SemanticResultValue("Search for", "Search");
            SemanticResultValue searchIn = new SemanticResultValue("Search in", "Search");

            // Create SemanticResultValue objects that contain category possibilities
            SemanticResultValue woman = new SemanticResultValue("woman", "womens");
            SemanticResultValue women = new SemanticResultValue("women", "womens");
            SemanticResultValue womens = new SemanticResultValue("womens", "womens");
            SemanticResultValue female = new SemanticResultValue("female", "womens");
            SemanticResultValue male = new SemanticResultValue("male", "mens");
            SemanticResultValue man = new SemanticResultValue("man", "mens");
            SemanticResultValue men = new SemanticResultValue("men", "mens");
            SemanticResultValue mens = new SemanticResultValue("mens", "mens");

            // Create Search
            Choices choicesSearch = new Choices();
            choicesSearch.Add(new Choices(new GrammarBuilder[] { search, searchFor, searchIn }));

            //Create Categories
            Choices choicesCategories = new Choices();
            choicesCategories.Add(new Choices(new GrammarBuilder[] { woman, women, womens, female, male, man, men, mens }));

            // build category search
            GrammarBuilder grammarSearchCategories = new GrammarBuilder();
            grammarSearchCategories.Append(new SemanticResultKey("activator", choicesSearch));
            grammarSearchCategories.Append(new SemanticResultKey("category", choicesCategories));

            SearchCatGrammar = new Grammar(grammarSearchCategories);

            List<SemanticResultValue> arraySubCatValue = new List<SemanticResultValue>();

            foreach(Models.Category category in Collector.Categories)
                foreach(Models.SubCategory subCategory in category.SubCategories)
                    arraySubCatValue.Add(new SemanticResultValue(subCategory.Title.Replace("&", "and"), subCategory.Title));

            Choices choicesSubCategories = new Choices();
            foreach(SemanticResultValue semValue in arraySubCatValue)
            {
                choicesSubCategories.Add(new Choices(new GrammarBuilder(semValue)));
            }

            GrammarBuilder grammarSubCat = new GrammarBuilder();
            grammarSubCat.Append(new SemanticResultKey("activator", choicesSearch));
            grammarSubCat.Append(new SemanticResultKey("category", choicesCategories));
            grammarSubCat.Append(new SemanticResultKey("subcategory", choicesSubCategories));

            SearchSubCatGrammar = new Grammar(grammarSubCat);

            // Create SemanticResultValue objects that contain wishlist possibilities
            SemanticResultValue addToWishlist = new SemanticResultValue("Add to the wishlist", "Add to Wishlist");
            SemanticResultValue addToWishlist2 = new SemanticResultValue("Add to my wishlist", "Add to Wishlist");
            SemanticResultValue removeFromWishlist = new SemanticResultValue("Remove from my wishlist", "Remove from Wishlist");
            SemanticResultValue add = new SemanticResultValue("add", "Add to Wishlist");
            SemanticResultValue remove = new SemanticResultValue("remove", "Remove from Wishlist");
            SemanticResultValue removeFromWishlist2 = new SemanticResultValue("Remove from  the wishlist", "Remove from Wishlist");

            // Create Activator 'choices'
            Choices choicesWishlist = new Choices();
            choicesWishlist.Add(new Choices(new GrammarBuilder[] { addToWishlist, addToWishlist2, removeFromWishlist, removeFromWishlist2 }));

            // Build the phrase and add 'choices'
            GrammarBuilder grammarWishlist = new GrammarBuilder();
            grammarWishlist.Append(new SemanticResultKey("activator", choicesWishlist));

            // Build a Grammar object from the GrammarBuilder.
            WishListGrammar = new Grammar(grammarWishlist);

            IsListening = false;
        }

        private void EnableSpeechGrammars()
        {
            speechEngine.UnloadAllGrammars();
            speechEngine.LoadGrammarAsync(ActivateGrammar);

            if (IsListening)
            {
                // nav grammar
                speechEngine.LoadGrammarAsync(NavigateGrammar);

                //do search page grammar
                speechEngine.LoadGrammarAsync(SearchCatGrammar);
                speechEngine.LoadGrammarAsync(SearchSubCatGrammar);

                if (IsProduct)
                {
                    // do wishlist grammar
                    speechEngine.LoadGrammarAsync(WishListGrammar);
                }
            }
        }

        void speechEngine_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            foreach(RecognizedPhrase phrase in e.Result.Alternates)
            {
                MessageBox.Show("Words: " + phrase.Text + "\r\nConfidence: " + phrase.Confidence.ToString());
            }
        }

        void speechEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        { 
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                try
                {
                    if(e.Result.Semantics["activator"].Value.ToString() == "Start Listening")
                    {
                        IsListening = true;
                        this.ImNotListeningBox.Visibility = Visibility.Hidden;
                        this.ImListeningBox.Visibility = Visibility.Visible;
                    }
                    else if(e.Result.Semantics["activator"].Value.ToString() == "Stop Listening")
                    {
                        IsListening = false;
                        this.ImListeningBox.Visibility = Visibility.Hidden;
                        this.ImNotListeningBox.Visibility = Visibility.Visible;
                    }
                    else if (e.Result.Semantics["activator"].Value.ToString() == "Navigate")
                    {
                        string searchTerm = e.Result.Semantics["where"].Value.ToString();

                        UserControl userControl;

                        switch(searchTerm)
                        {
                            case "Shop":
                                userControl = new Views.Category();
                                break;
                            case "Account":
                                userControl = new Account();
                                break;
                            case "Wishlist":
                                userControl = new Wishlist();
                                break;
                            case "Search":
                                userControl = new Search();
                                break;
                            case "Home":
                                userControl = new LandingPage();
                                break;
                            default:
                                userControl = null;
                                break;
                        }

                        if (userControl != null)
                            Switcher.Switch(userControl);
                        else
                            MessageBox.Show("The Search Term failed (coding error, soz... ignore this bit:) \r\n " + searchTerm + " : " + e.Result.Semantics["activator"].Value.ToString());
                    }
                    else if(e.Result.Semantics["activator"].Value.ToString() == "Search")
                    {
                        Collector.lastSearchTerm = "";
                        Collector.Search = new ObservableCollection<Item>();

                        string category = e.Result.Semantics["category"].Value.ToString();
                        string subCategory;
                        try
                        {
                            subCategory = e.Result.Semantics["subcategory"].Value.ToString();
                            //yes subcategory
                            Models.SubCategory subCategoryFound = Collector.GetSubCategory(subCategory, category);
                            if (subCategoryFound != null)
                            {
                                if (!IsSearch)
                                    Switcher.Switch(new Search());
                                foreach (Item product in subCategoryFound.Products)
                                    Collector.Search.Add(product);
                            }
                            Collector.lastSearchTerm = category.ToUpper() + " " + subCategory.ToUpper();
                        }
                        catch
                        {
                            //no subcategory
                            Models.Category categoryFound = Collector.GetCategory(category);
                            if (categoryFound != null)
                            {
                                if(!IsSearch)
                                    Switcher.Switch(new Search());
                                foreach (Models.SubCategory subCat in categoryFound.SubCategories)
                                    foreach (Item product in subCat.Products)
                                        Collector.Search.Add(product);
                            }
                            Collector.lastSearchTerm = category.ToUpper();
                        }
                    }
                    else if (e.Result.Semantics["activator"].Value.ToString() == "Add to Wishlist")
                    {
                        if (!Collector.Wishlist.Contains(Product))
                        {
                            Collector.Wishlist.Add(Product);
                            Switcher.Switch(new Product(Product, Collector.GetSubCategory(Product.ParentSub, Product.ParentCat), Collector.GetCategory(Product.ParentCat)));
                        }

                    }
                    else if (e.Result.Semantics["activator"].Value.ToString() == "Remove from Wishlist")
                    {
                        if (Collector.Wishlist.Contains(Product))
                        {
                            Collector.Wishlist.Remove(Product);
                            Switcher.Switch(new Product(Product, Collector.GetSubCategory(Product.ParentSub, Product.ParentCat), Collector.GetCategory(Product.ParentCat)));
                        }
                    }
                }
                catch
                {

                }
            }
        }
        #endregion

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
                }
                catch (Exception e)
                {
                }
            }

            return;
        }

        private double GetShoulderWidth(Skeleton skel)
        {
            /* Get the points of the right and left shoulders */
            CoordinateMapper coordMapper = new CoordinateMapper(sensorChooser.Kinect);
            DepthImagePoint rightShoulder = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.ShoulderRight].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint leftShoulder = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.ShoulderLeft].Position, DepthImageFormat.Resolution640x480Fps30);

            if (rightShoulder == null || leftShoulder == null)
                throw new ArgumentNullException("Could not find one or both shoulders");

            return Math.Sqrt(Math.Pow(Math.Abs(rightShoulder.X - leftShoulder.X), 2) + Math.Pow(Math.Abs(rightShoulder.Y - leftShoulder.Y), 2));
        }

        private double GetWaistWidth(Skeleton skel)
        {
            /* Get the points of the right and left shoulders */
            CoordinateMapper coordMapper = new CoordinateMapper(sensorChooser.Kinect);
            DepthImagePoint rightHip = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.HipRight].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint leftHip = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.HipLeft].Position, DepthImageFormat.Resolution640x480Fps30);

            if (rightHip == null || leftHip == null)
                throw new ArgumentNullException("Could not find one or both shoulders");

            return Math.Sqrt(Math.Pow(Math.Abs(rightHip.X - leftHip.X), 2) + Math.Pow(Math.Abs(rightHip.Y - leftHip.Y), 2));
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

        private void DisplayProductOnUser(Skeleton skel, DrawingContext dc)
        {
            if (skel == null)
                throw new ArgumentNullException("The skeleton was null upon trying to place product on the user");

            /* Get the coords of the body */
            CoordinateMapper coordMapper = new CoordinateMapper(sensorChooser.Kinect);
            DepthImagePoint head = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.Head].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint centerShoulder = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.ShoulderCenter].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint rightShoulder = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.ShoulderRight].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint leftShoulder = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.ShoulderLeft].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint rightElbow = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.ElbowRight].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint leftElbow = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.ElbowLeft].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint rightHip = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.HipRight].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint centerHip = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.HipCenter].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint leftHip = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.HipLeft].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint rightKnee = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.KneeRight].Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint leftKnee = coordMapper.MapSkeletonPointToDepthPoint(skel.Joints[JointType.KneeLeft].Position, DepthImageFormat.Resolution640x480Fps30);

            ImageSource image = new BitmapImage(new Uri(Product.ProductImage + ".png"));
            DrawingGroup drawingGroup = new DrawingGroup();
            double angleRotate = GetAngle(centerHip, centerShoulder);

            double imageHeight = image.Height;
            double imageWidth = image.Width;

            double ratio = imageHeight / imageWidth;
            
            // create variables
            Point imagePoint = new Point();
            double x, y, xcoord, ycoord, width;

            if (Product.ParentSub.ToLower() == "jeans" || Product.ParentSub.ToLower() == "shorts" || Product.ParentSub.ToLower() == "skirts"
                || (Product.ParentCat.ToLower() == "mens" && Product.ParentSub.ToLower() == "suits" && Product.Title.ToLower().Contains("trouser")))
            {
                centerHip.Y += 50;

                if (Product.ParentSub.ToLower() == "skirts" &&
                    Product.IsThin)
                    width = GetShoulderWidth(skel) * 1.5;
                else
                    width = GetShoulderWidth(skel) * 1.2;

                imagePoint = GetXYCoordToUse(centerHip, width, angleRotate);
                x = imagePoint.X;
                y = imagePoint.Y;

                if (Math.Abs(leftHip.Depth - rightHip.Depth) < 10)
                {
                    ClothingHeight = ratio * width;
                }

                xcoord = centerHip.X;
                ycoord = centerHip.Y;
            }
            else
            {
                centerShoulder.Y += 10;

                if (Product.IsThin)
                    width = GetShoulderWidth(skel);
                else
                    width = GetShoulderWidth(skel) * 1.5;

                imagePoint = GetXYCoordToUse(centerShoulder, width, angleRotate);
                x = imagePoint.X;
                y = imagePoint.Y;

                if (Math.Abs(leftShoulder.Depth - rightShoulder.Depth) < 10)
                {
                    ClothingHeight = ratio * width;
                }

                xcoord = centerShoulder.X;
                ycoord = centerShoulder.Y;
            }

            if (angleRotate > 0)
            {
                x = x + (angleRotate * 1.15);
            }
            else if (angleRotate < 0)
            {
                x = x - (Math.Abs(angleRotate) * 1.15);
            }

            if (ClothingHeight != 0)
            {
                drawingGroup.Transform = new RotateTransform(angleRotate, xcoord, ycoord - 50);
                drawingGroup.Children.Add(new ImageDrawing(image, new Rect(x, y, width, ClothingHeight)));
                dc.DrawDrawing(drawingGroup);
            }
            else
            {
                dc.DrawText(new FormattedText("Please stand straight for configuration", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Segoe"), 24, Brushes.White), new Point(centerShoulder.X - 200, centerShoulder.Y + 100));
            }
        }

        private Point GetXYCoordToUse(DepthImagePoint pointToUse, double width, double angle)
        {
            Point point = new Point();
            point.X = pointToUse.X - (width / 2);

            double x = (180 - angle) / 2;
            double h = Math.Sin(x) * (width * Math.PI/180) * 2 * Math.Cos(x);

            point.Y = pointToUse.Y + h;

            return point;
        }

        private double GetAngle(DepthImagePoint centerHip, DepthImagePoint centerShoulder)
        {
            double w = centerShoulder.X - centerHip.X;
            double h = centerShoulder.Y - centerHip.Y;

            if (w == 0)
                w = 0.0000000000000001;

            var atan = Math.Atan(h / w) / Math.PI * 180;
            if (w < 0 && h <= 0)
                atan -= 180;
            if (w < 0 && h > 0)
                atan += 180;
            return (atan + 90) % 360;
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
                            //this.DrawBonesAndJoints(skel, dc);

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
            catch
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

                    args.OldSensor.AudioSource.Stop();

                    this.speechEngine.SpeechRecognized -= speechEngine_SpeechRecognized;
                    this.speechEngine.SpeechRecognitionRejected -= speechEngine_SpeechRecognitionRejected;
                    this.speechEngine.RecognizeAsyncStop();
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

                    this.speechEngine.SpeechRecognized += speechEngine_SpeechRecognized;
                    this.speechEngine.SpeechRecognitionRejected += speechEngine_SpeechRecognitionRejected;
                    this.speechEngine.SetInputToAudioStream(args.NewSensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));

                    this.speechEngine.RecognizeAsync(RecognizeMode.Multiple);
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
            if (nextPage.GetType() == typeof(Product))
            {
                Product = ((Product)nextPage).CurrentProduct;
                IsProduct = true;
            }
            else if (nextPage.GetType() == typeof(LandingPage))
            {
                Product = ((LandingPage)nextPage).CurrentProduct;
                IsProduct = false;
            }
            else if (nextPage.GetType() == typeof(Search))
            {
                Product = null;
                Collector.Search.Clear();
                Collector.lastSearchTerm = "";
                IsProduct = false;
                IsSearch = true;
            }
            else
            {
                Product = null;
                IsProduct = false;
            }

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
