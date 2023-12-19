/******************************************************************************************************************************************************
* MIT License																																		  *
*																																					  *
* Copyright (c) 2021																																  *
* Emmanuel Badier <emmanuel.badier@gmail.com>																										  *
* 																																					  *
* Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),  *
* to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,  *
* and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:          *
* 																																					  *
* The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.					  *
* 																																					  *
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, *
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 																							  *
* IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 		  *
* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.							  *
******************************************************************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.UI;

namespace MMBTS
{
	public sealed class SerialPort_MMBTS_Test : MonoBehaviour
	{
		public TMPro.TMP_Dropdown serialPort_Dropdown;
		public Slider triggersValue_Slider;
		public Slider triggersDuration_Slider;
		public Slider triggersInterval_Slider;
		public Button connectSerialPort_Button;
		public Button startStopTriggers_Button;
		public Toggle logTriggers_Toggle;

		private Text _triggersValue_Text;
		private Text _triggersDuration_Text;
		private Text _triggersInterval_Text;

		private TMPro.TMP_Text _connectSerialPort_Button_Text;
		private Image _connectSerialPort_Button_Image;
		private Color _connectSerialPort_Button_OriginalColor;

		private TMPro.TMP_Text _startStopTriggers_Button_Text;
		private Image _startStopTriggers_Button_Image;
		private Color _startStopTriggers_Button_OriginalColor;

		private Boolean _isConnected = false;
		private Coroutine _triggersLoop = null;
		private SerialPort_MMBTS _serialPort_MMBTS = new SerialPort_MMBTS();

		private void Awake()
		{
			serialPort_Dropdown.interactable = true;

			connectSerialPort_Button.onClick.AddListener(_Connect);
			connectSerialPort_Button.interactable = true;

			_triggersValue_Text = triggersValue_Slider.GetComponentInChildren<Text>();
			triggersValue_Slider.onValueChanged.AddListener((float val) => _UpdateSliderText(val, "0", _triggersValue_Text));
			triggersValue_Slider.wholeNumbers = true;
			triggersValue_Slider.maxValue = byte.MaxValue;
			triggersValue_Slider.value = triggersValue_Slider.minValue = 1;

			_triggersDuration_Text = triggersDuration_Slider.GetComponentInChildren<Text>();
			triggersDuration_Slider.onValueChanged.AddListener((float val) => _UpdateSliderText(val, "0.00", _triggersDuration_Text));
			triggersDuration_Slider.wholeNumbers = false;
			triggersDuration_Slider.maxValue = 2f;
			triggersDuration_Slider.minValue = 0.01f;
			triggersDuration_Slider.value = 0.05f;

			_triggersInterval_Text = triggersInterval_Slider.GetComponentInChildren<Text>();
			triggersInterval_Slider.onValueChanged.AddListener((float val) => _UpdateSliderText(val, "0.00", _triggersInterval_Text));
			triggersInterval_Slider.wholeNumbers = false;
			triggersInterval_Slider.maxValue = 2f;
			triggersInterval_Slider.minValue = 0.01f;
			triggersInterval_Slider.value = 0.50f;

			_connectSerialPort_Button_Text = connectSerialPort_Button.GetComponentInChildren<TMPro.TMP_Text>();
			_connectSerialPort_Button_Image = connectSerialPort_Button.GetComponentInChildren<Image>();
			_connectSerialPort_Button_OriginalColor = _connectSerialPort_Button_Image.color;

			_startStopTriggers_Button_Text = startStopTriggers_Button.GetComponentInChildren<TMPro.TMP_Text>();
			_startStopTriggers_Button_Image = startStopTriggers_Button.GetComponent<Image>();
			_startStopTriggers_Button_OriginalColor = _startStopTriggers_Button_Image.color;
			startStopTriggers_Button.onClick.AddListener(_StartStopTriggersLoop);
			startStopTriggers_Button.interactable = false;

			logTriggers_Toggle.isOn = false;
		}

        private void Start()
		{
			_ListSerialPorts();			
		}

		private void OnDestroy()
		{
			if (_triggersLoop != null)
			{
				_StopTriggersLoop();
			}

			if (_isConnected)
            {
				_Disconnect();
            }
		}

		private void _ListSerialPorts()
		{
			string[] availableSerialPorts = SerialPort.GetPortNames();
			List<string> serialPortNames = new List<string>();

			if (availableSerialPorts.Length > 0)
			{
				serialPortNames.AddRange(availableSerialPorts);
			}

			else
			{
				serialPortNames.Add("N/A");
				serialPort_Dropdown.interactable = false;
			}

			string msg = "[SerialPort_MMBTS_Test] Available ports: ";
			foreach (string port in serialPortNames)
			{
				msg += port + " ; ";
			}
			Debug.Log(msg);

			serialPort_Dropdown.ClearOptions();
			serialPort_Dropdown.AddOptions(serialPortNames);
			serialPort_Dropdown.RefreshShownValue();
		}

		private void _Connect()
		{
			string msg;

			if (!_isConnected)
			{
				if (_serialPort_MMBTS.Connect(out msg, serialPort_Dropdown.captionText.text))
				{
					serialPort_Dropdown.interactable = false;
					startStopTriggers_Button.interactable = true;
					_connectSerialPort_Button_Text.text = "Disconnect";
					_connectSerialPort_Button_Image.color = Color.red;
					_isConnected = true;
					Debug.Log(msg);
				}
				else
				{
					startStopTriggers_Button.interactable = false;
					_isConnected = false;
					Debug.LogWarning(msg);
				}
			}

			else
			{
				_Disconnect();
				serialPort_Dropdown.interactable = true;
				_connectSerialPort_Button_Text.text = "Connect";
				_connectSerialPort_Button_Image.color = _connectSerialPort_Button_OriginalColor;
				startStopTriggers_Button.interactable = false;
				_isConnected = false;
			}
		}

		private void _Disconnect()
		{
			if (_isConnected)
			{
				_serialPort_MMBTS.Disconnect();
			}
		}

		private void _UpdateSliderText(float pVal, string pFormat, Text pText)
		{
			pText.text = pVal.ToString(pFormat);
		}

		private void _StartStopTriggersLoop()
		{
			if (_triggersLoop != null)
			{
				_StopTriggersLoop();
				_startStopTriggers_Button_Text.text = "Start sending triggers";
				_startStopTriggers_Button_Image.color = _startStopTriggers_Button_OriginalColor;
				connectSerialPort_Button.interactable = true;
			}
			else
			{
				_triggersLoop = StartCoroutine(_SendTriggersLoopCoroutine());
				_startStopTriggers_Button_Text.text = "Stop sending triggers";
				_startStopTriggers_Button_Image.color = Color.red;
				serialPort_Dropdown.interactable = false;
				connectSerialPort_Button.interactable = false;
			}
		}

		private void _StopTriggersLoop()
		{
			StopCoroutine(_triggersLoop);
			_triggersLoop = null;
			_serialPort_MMBTS.SendTrigger(byte.MinValue);
		}

		private IEnumerator _SendTriggersLoopCoroutine()
		{
			while (true)
			{
				bool log = logTriggers_Toggle.isOn;
				byte trigger = (byte)triggersValue_Slider.value;
				
				_serialPort_MMBTS.SendTrigger(trigger);
				if(log)
				{
					Debug.Log("[SerialPort_MMBTS_Test] Trigger sent : " + trigger);
				}

				yield return new WaitForSecondsRealtime(triggersDuration_Slider.value);
				
				_serialPort_MMBTS.SendTrigger(byte.MinValue);
				if (log)
				{
					Debug.Log("[SerialPort_MMBTS_Test] Trigger reset (0)");
				}

				yield return new WaitForSecondsRealtime(triggersInterval_Slider.value);
			}
		}
	}
}