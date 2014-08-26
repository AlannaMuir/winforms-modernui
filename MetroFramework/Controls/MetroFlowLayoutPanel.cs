using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace MetroFramework.Controls
{
    // This class demonstrates a simple custom layout engine. 
    public class FlowLayout : LayoutEngine
    {
        public FlowDirection FlowDirection = FlowDirection.LeftToRight;
        public Boolean WrapContents = true;

        public override bool Layout(
            object container,
            LayoutEventArgs layoutEventArgs)
        {
            Control parent = container as Control;
            Int32 width = parent.Width;

            // Use DisplayRectangle so that parent.Padding is honored.
            Rectangle parentDisplayRectangle = parent.DisplayRectangle;
            Point nextControlLocation = parentDisplayRectangle.Location;

            foreach (Control c in parent.Controls)
            {
                // Only apply layout to visible controls. 
                if ((!c.Visible) || (c.GetType() == typeof(MetroScrollBar)))
                {
                    continue;
                }

                // Respect the margin of the control: 
                // shift over the left and the top.
                //nextControlLocation.Offset(c.Margin.Left, c.Margin.Top);

                if (nextControlLocation.X + c.Width > width)
                {
                    nextControlLocation.X = parentDisplayRectangle.X;
                    nextControlLocation.Y += c.Height + c.Margin.Bottom;
                }

                // Set the location of the control.
                c.Location = nextControlLocation;

                // Set the autosized controls to their  
                // autosized heights. 
                if (c.AutoSize)
                {
                    c.Size = c.GetPreferredSize(parentDisplayRectangle.Size);
                }

                nextControlLocation.X += c.Width + c.Margin.Right;

                // Move X back to the display rectangle origin.
                //nextControlLocation.X = parentDisplayRectangle.X;

                // Increment Y by the height of the control  
                // and the bottom margin.
                //nextControlLocation.Y += c.Height + c.Margin.Bottom;
            }

            // Optional: Return whether or not the container's  
            // parent should perform layout as a result of this  
            // layout. Some layout engines return the value of  
            // the container's AutoSize property. 

            return true;
        }

        public static readonly FlowLayout Instance = new FlowLayout();

        private readonly Dictionary<Control, Boolean> flowBreaks =
            new Dictionary<Control, Boolean>();

        internal bool GetFlowBreak(Control control)
        {
            if (flowBreaks.ContainsKey(control))
                return flowBreaks[control];
            return false;
        }

        internal void SetFlowBreak(Control control, bool value)
        {
            flowBreaks[control] = value;
        }
    }

    public class MetroFlowLayoutPanel : MetroPanel, IExtenderProvider
    {
        private readonly FlowLayout layout = FlowLayout.Instance;
        public override LayoutEngine LayoutEngine { get { return layout; } }

        //[SRDescription(SR.FlowPanelFlowDirectionDescr)]
        [DefaultValue(FlowDirection.LeftToRight)]
        //[SRCategory(SR.CatLayout)]
        [Localizable(true)]
        public FlowDirection FlowDirection
        {
            get { return layout.FlowDirection; }
            set
            {
                layout.FlowDirection = value;
                Debug.Assert(FlowDirection == value, "FlowDirection should be the same as we set it");
            }
        }

        //[SRDescription(SR.FlowPanelWrapContentsDescr)]
        [DefaultValue(true)]
        //[SRCategory(SR.CatLayout)]
        [Localizable(true)]
        public bool WrapContents
        {
            get { return layout.WrapContents; }
            set
            {
                layout.WrapContents = value;
                Debug.Assert(WrapContents == value, "WrapContents should be the same as we set it");
            }
        }

        bool IExtenderProvider.CanExtend(object obj)
        {
            Control control = obj as Control;
            return control != null && control.Parent == this;
        }

        [DefaultValue(false)]
        [DisplayName("FlowBreak")]
        public bool GetFlowBreak(Control control)
        {
            return layout.GetFlowBreak(control);
        }

        [DisplayName("FlowBreak")]
        public void SetFlowBreak(Control control, bool value)
        {
            layout.SetFlowBreak(control, value);
        }

        public MetroFlowLayoutPanel()
        {
            //_flowLayoutSettings = FlowLayout.CreateSettings(this);
        }
    }
}
