using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.AdminClient
{
    /// <summary>
    /// This is a special ComboBox that each item may conatins an image.
    /// </summary>
    public class ImagedComboBox : ComboBox
    {
        private ComboCollection<ComboBoxItem> _items;

        /// <summary>
        /// The imaged ComboBox items.
        /// this property is invisibile for design serializer.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ComboCollection<ComboBoxItem> Items
        {
            get { return _items; }
            set { _items = value; }
        }


        public ImagedComboBox()
        {
            //Specifies that the list is displayed by clicking the down arrow and that the text portion is not editable. 
            //This means that the user cannot enter a new value. 
            //Only values already in the list can be selected. The list displays only if 
            DropDownStyle = ComboBoxStyle.DropDownList;
            //All the elements in the control are drawn manually and can differ in size.
            DrawMode = DrawMode.OwnerDrawVariable;
            //using DrawItem event we need to draw item
            DrawItem += ComboBoxDrawItemEvent;
            MeasureItem += ComboBox1_MeasureItem;



        }


        protected override ControlCollection CreateControlsInstance()
        {
            _items = new ComboCollection<ComboBoxItem>
            {
                ItemsBase = base.Items
            };

            _items.UpdateItems += UpdateItems;

            return base.CreateControlsInstance();
        }



        /// <summary>
        /// Handles UpdateItems event which fired when an item, added, removed or inserted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateItems(object sender, EventArgs e)
        {
        }


        /// <summary>
        /// I have set the Draw property to DrawMode.OwnerDrawVariable, so I must caluclate the item measurement.  
        /// I will set the height and width of each item before it is drawn. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            var g = CreateGraphics();
            var maxWidth = 0;
            foreach (var width in Items.ItemsBase.Cast<object>().Select(element => (int)g.MeasureString(element.ToString(), Font).Width).Where(width => width > maxWidth))
            {
                maxWidth = width;
            }
            DropDownWidth = maxWidth + 20;
        }



        /// <summary>
        /// Draws overrided items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxDrawItemEvent(object sender, DrawItemEventArgs e)
        {
            //Draw backgroud of the item
            e.DrawBackground();
            if (e.Index != -1)
            {
                var comboboxItem = Items[e.Index];
                //Draw the image in combo box using its bound and ItemHeight
                e.Graphics.DrawImage(comboboxItem.Image, e.Bounds.X, e.Bounds.Y, ItemHeight, ItemHeight);

                //we need to draw the item as string because we made drawmode to ownervariable
                e.Graphics.DrawString(Items[e.Index].Value.ToString(), Font, Brushes.Black,
                                      new RectangleF(e.Bounds.X + ItemHeight, e.Bounds.Y, DropDownWidth,
                                                     ItemHeight));
            }
            //draw rectangle over the item selected
            e.DrawFocusRectangle();
        }

    }
}
