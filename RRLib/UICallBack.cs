using System;

namespace RRLib
{
	public delegate void UpdateUIHandler(object param, object param2);

	public class UICallBack
	{
		public UpdateUIHandler updateUI;
		public object param;

		public UICallBack(UpdateUIHandler _updateUI, object _param)
		{
			updateUI = _updateUI;
			param = _param;
		}

		public void Invoke(object param2)
		{
			updateUI(param, param2);
		}

		public void Invoke()
		{
			updateUI(param, null);
		}
	}
}
