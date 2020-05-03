using System.Windows;
using System.Windows.Controls;

namespace System_Engineer
{
	/// <summary>
	/// Interaction logic for scanButtom.xaml
	/// </summary>
	public partial class scanButtom : UserControl
	{
		public scanButtom()
		{
			this.InitializeComponent();
		}

		private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			VisualStateManager.GoToState(this, "MouseOver",true);
		}

		private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			VisualStateManager.GoToState(this, "Normal",true);
		}

		
	}
}