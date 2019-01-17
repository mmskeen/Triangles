using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Triangles
{
    /// <summary>
    /// MainPage of the Triangles program.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Polygon triangle;

        public MainPage()
        {
            this.InitializeComponent();
            UpdateTriangleDescription();
        }

        /// <summary>
        /// Used to only allow numeric input in triangle length textboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            string key = e.Key.ToString();
            if (key.Equals("Back"))
            {
                e.Handled = false;
                return;
            }
            if (key.Equals("190"))  // a decimal (or period) symbol is 190
            {
                TextBox box = (TextBox)sender;
                if (box.Text.IndexOf(".") == -1)
                {
                    e.Handled = false;
                    return;
                }
            }
            for (int i = 0; i < 10; i++)
            {
                if (key == string.Format("Number{0}", i))
                {
                    e.Handled = false;
                    return;
                }
            }
            e.Handled = true;
        }

        /// <summary>
        /// Upon changing a textbox, make calculations and update triangle description
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTriangleDescription();
        }

        /// <summary>
        /// Update the triangle description
        /// </summary>
        private void UpdateTriangleDescription()
        {
            layoutRoot.Children.Remove(triangle);   // clear out any previously drawn triangle
            textBlockResult.Text = "These side lengths produce a" + CalculateTriangleType() + " triangle.";
        }

        /// <summary>
        /// Calculate the type of triangle (right, invalid, etc.)
        /// </summary>
        /// <returns>String describing triangle type</returns>
        private string CalculateTriangleType()
        {
            double a, b, c;
            const double nearlyZero = 0.00000001;   // used as a "zero equivalent" to calculate right triangles for floats
            string triangleType = " valid";         // default case is normal triangle

            // Check that all textboxes have valid input
            if (!double.TryParse(textBoxA.Text, out a) || !double.TryParse(textBoxB.Text, out b) || !double.TryParse(textBoxC.Text, out c))
            {
                triangleType = "n invalid";
                return triangleType;
            }

            // Check if the side lengths form a valid triangle
            if ((a + b < c || a + c < b || b + c < a) || (a == 0 || b == 0 || c == 0))
            {
                triangleType = "n invalid";
                return triangleType;
            }

            // Check what kind of triangle it is
            if (a == b || a == c || b == c)
            {
                triangleType = (a == b && a == c) ? " valid equilateral" : " valid isosceles";
            }
            if (Math.Abs(c * c - a * a - b * b) < nearlyZero || Math.Abs(a * a - b * b - c * c) < nearlyZero
                || Math.Abs(b * b - a * a - c * c) < nearlyZero)
            {
                triangleType = triangleType.Equals(" valid isosceles") ? " valid right isosceles" : " valid right";
            }

            DrawTriangle(a, b, c, triangleType);
            return triangleType;
        }

        private void DrawTriangle(double a, double b, double c, string triangleType)
        {
            // Used to calculate the y coordinate of the textBlockResult box, to place the triangle
            var ttv = textBlockResult.TransformToVisual(Windows.UI.Xaml.Window.Current.Content);
            Point textBlockCoords = ttv.TransformPoint(new Point(0, 0));

            // Find the available room we have to place the triangle, and find the right origin
            double maxLength = (((Frame)Windows.UI.Xaml.Window.Current.Content).ActualHeight - textBlockCoords.Y) * 0.85;
            double minLength = maxLength / 5;
            double originX = ((Frame)Windows.UI.Xaml.Window.Current.Content).ActualWidth * 0.5;
            double originY = ((Frame)Windows.UI.Xaml.Window.Current.Content).ActualHeight * 0.97;
            double scale = 1;

            // Scale the triangle to the available room we have, if necessary
            double longestSide = Math.Max(Math.Max(a, b), c);
            if (longestSide < minLength || longestSide > maxLength)
            {
                scale = longestSide / maxLength;
            }
            a = a / scale;
            b = b / scale;
            c = c / scale;

            Point p1 = new Point(originX - a  /2, originY);
            Point p2 = new Point(originX + a / 2, originY);

            // used geometry to find the proper forumulas to create the third point coordinates
            double x = (a * a + b * b - c * c) / 2 / a;
            double y = Math.Sqrt(b * b - x * x);
            Point p3 = new Point(originX - a / 2 + x, originY - y);

            // Assign the triangle a color based off its type
            Windows.UI.Color color = triangleType.Contains("right isosceles") ? Windows.UI.Colors.Aqua
                : triangleType.Contains("isosceles") ? Windows.UI.Colors.BlueViolet
                : triangleType.Contains("right") ? Windows.UI.Colors.LightBlue
                : triangleType.Contains("equilateral") ? Windows.UI.Colors.LightGoldenrodYellow
                : Windows.UI.Colors.MintCream;

            triangle = new Polygon
            {
                Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
                Fill = new SolidColorBrush(color),
                Points = new PointCollection { p1, p2, p3 }
            };
            layoutRoot.Children.Add(triangle);
        }
    }
}
