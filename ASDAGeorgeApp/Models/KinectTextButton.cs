using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ASDAGeorgeApp.Models
{
    internal class KinectTextButton : KinectButtonBase
    {
        /// <summary>
        /// IsHandPointerOver dependency property for use in the control template triggers
        /// </summary>
        public static readonly DependencyProperty IsHandPointerOverProperty = DependencyProperty.Register(
            "IsHandPointerOver", typeof(bool), typeof(KinectTextButton), new PropertyMetadata(false));

        /// <summary>
        /// Boolean value to tell us if the control is being displayed in the Visual Studio designer
        /// </summary>
        private static readonly bool IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

        private HandPointer activeHandpointer;

        public KinectTextButton()
        {
            this.InitializeKinectTextButton();
        }

        protected override void OnClick()
        {
            base.OnClick();
        }

        /// <summary>
        /// Boolean value that returns true if a mouse or hand pointer is over this button
        /// </summary>
        public bool IsHandPointerOver
        {
            get
            {
                return (bool)this.GetValue(IsHandPointerOverProperty);
            }

            set
            {
                this.SetValue(IsHandPointerOverProperty, value);
            }
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.IsHandPointerOver = true;
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.IsHandPointerOver = false;
        }

        private void InitializeKinectTextButton()
        {
            KinectRegion.AddHandPointerEnterHandler(this, this.OnHandPointerEnter);
            KinectRegion.AddHandPointerLeaveHandler(this, this.OnHandPointerLeave);
        }

        private void OnHandPointerEnter(object sender, HandPointerEventArgs e)
        {
            if (!e.HandPointer.IsPrimaryHandOfUser || !e.HandPointer.IsPrimaryUser)
            {
                return;
            }

            this.activeHandpointer = e.HandPointer;
            this.IsHandPointerOver = true;
        }

        private void OnHandPointerLeave(object sender, HandPointerEventArgs e)
        {
            if (this.activeHandpointer != e.HandPointer)
            {
                return;
            }

            this.activeHandpointer = null;
            this.IsHandPointerOver = false;
        }
    }
}
