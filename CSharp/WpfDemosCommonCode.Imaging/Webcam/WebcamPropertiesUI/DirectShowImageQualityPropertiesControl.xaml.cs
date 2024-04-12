using System.Windows;
using System.Windows.Controls;

using Vintasoft.Imaging.Media;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A control that allows to change the image quality properties of webcam.
    /// </summary>
    public partial class DirectShowImageQualityPropertiesControl : UserControl
    {

        #region Structs

        /// <summary>
        /// Stores information about values of the image quality property.
        /// </summary>
        class ImageQualityPropertyInfo
        {
            internal bool Auto;
            internal int CurrentValue;
            internal int DefaultValue;
            internal int MinValue;
            internal int MaxValue;
            internal int StepSize;


            internal ImageQualityPropertyInfo(
                DirectShowImageQualityPropertyValue currentValue,
                int defaultValue,
                int minValue,
                int maxValue,
                int stepSize)
            {
                this.StepSize = stepSize;

                this.MinValue = minValue / stepSize;
                this.MaxValue = maxValue / stepSize;

                this.DefaultValue = GetValueInRange(defaultValue / stepSize);

                this.CurrentValue = GetValueInRange(currentValue.Value / stepSize);
                this.Auto = currentValue.Auto;
            }



            private int GetValueInRange(int value)
            {
                if (value < MinValue)
                    return MinValue;
                if (value > MaxValue)
                    return MaxValue;
                return value;
            }
        }

        #endregion



        #region Fields

        ImageQualityPropertyInfo _backlightCompensationPropertyInfo;
        ImageQualityPropertyInfo _brightnessPropertyInfo;
        ImageQualityPropertyInfo _colorEnablePropertyInfo;
        ImageQualityPropertyInfo _contrastPropertyInfo;
        ImageQualityPropertyInfo _gainPropertyInfo;
        ImageQualityPropertyInfo _gammaPropertyInfo;
        ImageQualityPropertyInfo _huePropertyInfo;
        ImageQualityPropertyInfo _saturationPropertyInfo;
        ImageQualityPropertyInfo _sharpnessPropertyInfo;
        ImageQualityPropertyInfo _whiteBalancePropertyInfo;

        bool _isInitialized;

        #endregion



        #region Constructors

        public DirectShowImageQualityPropertiesControl()
        {
            InitializeComponent();
        }

        #endregion



        #region Properties

        DirectShowCamera _camera;
        /// <summary>
        /// Webcam to manage.
        /// </summary>
        public DirectShowCamera Camera
        {
            get
            {
                return _camera;
            }
            set
            {
                _camera = value;
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Handles the Loaded event of UserControl object.
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_camera == null)
            {
                imageQualityPropertiesGroupBox.IsEnabled = false;
                return;
            }

            DirectShowImageQualityPropertyValue currentValue;
            int defaultValue, minValue, maxValue, stepSize;

            // backlight compensation
            try
            {
                _camera.ImageQuality.GetSupportedBacklightCompensationValues(out minValue, out maxValue, out stepSize, out defaultValue);
                currentValue = _camera.ImageQuality.BacklightCompensation;

                _backlightCompensationPropertyInfo = new ImageQualityPropertyInfo(currentValue, defaultValue, minValue, maxValue, stepSize);
                InitProperty(backlightCompensationSlider, backlightCompensationCheckBox, _backlightCompensationPropertyInfo);
            }
            catch (DirectShowCameraException)
            {
                DisableProperty(backlightCompensationLabel, backlightCompensationSlider, backlightCompensationCheckBox);
            }

            // brightness
            try
            {
                _camera.ImageQuality.GetSupportedBrightnessValues(out minValue, out maxValue, out stepSize, out defaultValue);
                currentValue = _camera.ImageQuality.Brightness;

                _brightnessPropertyInfo = new ImageQualityPropertyInfo(currentValue, defaultValue, minValue, maxValue, stepSize);
                InitProperty(brightnessSlider, brightnessCheckBox, _brightnessPropertyInfo);
            }
            catch (DirectShowCameraException)
            {
                DisableProperty(brightnessLabel, brightnessSlider, brightnessCheckBox);
            }

            // color enable
            try
            {
                _camera.ImageQuality.GetSupportedColorEnableValues(out minValue, out maxValue, out stepSize, out defaultValue);
                currentValue = _camera.ImageQuality.ColorEnable;

                _colorEnablePropertyInfo = new ImageQualityPropertyInfo(currentValue, defaultValue, minValue, maxValue, stepSize);
                InitProperty(colorEnableSlider, colorEnableCheckBox, _colorEnablePropertyInfo);
            }
            catch (DirectShowCameraException)
            {
                DisableProperty(colorEnableLabel, colorEnableSlider, colorEnableCheckBox);
            }

            // contrast
            try
            {
                _camera.ImageQuality.GetSupportedContrastValues(out minValue, out maxValue, out stepSize, out defaultValue);
                currentValue = _camera.ImageQuality.Contrast;

                _contrastPropertyInfo = new ImageQualityPropertyInfo(currentValue, defaultValue, minValue, maxValue, stepSize);
                InitProperty(contrastSlider, contrastCheckBox, _contrastPropertyInfo);
            }
            catch (DirectShowCameraException)
            {
                DisableProperty(contrastLabel, contrastSlider, contrastCheckBox);
            }

            // gain
            try
            {
                _camera.ImageQuality.GetSupportedGainValues(out minValue, out maxValue, out stepSize, out defaultValue);
                currentValue = _camera.ImageQuality.Gain;

                _gainPropertyInfo = new ImageQualityPropertyInfo(currentValue, defaultValue, minValue, maxValue, stepSize);
                InitProperty(gainSlider, gainCheckBox, _gainPropertyInfo);
            }
            catch (DirectShowCameraException)
            {
                DisableProperty(gainLabel, gainSlider, gainCheckBox);
            }

            // gamma
            try
            {
                _camera.ImageQuality.GetSupportedGammaValues(out minValue, out maxValue, out stepSize, out defaultValue);
                currentValue = _camera.ImageQuality.Gamma;

                _gammaPropertyInfo = new ImageQualityPropertyInfo(currentValue, defaultValue, minValue, maxValue, stepSize);
                InitProperty(gammaSlider, gammaCheckBox, _gammaPropertyInfo);
            }
            catch (DirectShowCameraException)
            {
                DisableProperty(gammaLabel, gammaSlider, gammaCheckBox);
            }

            // hue
            try
            {
                _camera.ImageQuality.GetSupportedHueValues(out minValue, out maxValue, out stepSize, out defaultValue);
                currentValue = _camera.ImageQuality.Hue;

                _huePropertyInfo = new ImageQualityPropertyInfo(currentValue, defaultValue, minValue, maxValue, stepSize);
                InitProperty(hueSlider, hueCheckBox, _huePropertyInfo);
            }
            catch (DirectShowCameraException)
            {
                DisableProperty(hueLabel, hueSlider, hueCheckBox);
            }

            // saturation
            try
            {
                _camera.ImageQuality.GetSupportedSaturationValues(out minValue, out maxValue, out stepSize, out defaultValue);
                currentValue = _camera.ImageQuality.Saturation;

                _saturationPropertyInfo = new ImageQualityPropertyInfo(currentValue, defaultValue, minValue, maxValue, stepSize);
                InitProperty(saturationSlider, saturationCheckBox, _saturationPropertyInfo);
            }
            catch (DirectShowCameraException)
            {
                DisableProperty(saturationLabel, saturationSlider, saturationCheckBox);
            }

            // sharpness
            try
            {
                _camera.ImageQuality.GetSupportedSharpnessValues(out minValue, out maxValue, out stepSize, out defaultValue);
                currentValue = _camera.ImageQuality.Sharpness;

                _sharpnessPropertyInfo = new ImageQualityPropertyInfo(currentValue, defaultValue, minValue, maxValue, stepSize);
                InitProperty(sharpnessSlider, sharpnessCheckBox, _sharpnessPropertyInfo);
            }
            catch (DirectShowCameraException)
            {
                DisableProperty(sharpnessLabel, sharpnessSlider, sharpnessCheckBox);
            }

            // white balance
            try
            {
                _camera.ImageQuality.GetSupportedWhiteBalanceValues(out minValue, out maxValue, out stepSize, out defaultValue);
                currentValue = _camera.ImageQuality.WhiteBalance;

                _whiteBalancePropertyInfo = new ImageQualityPropertyInfo(currentValue, defaultValue, minValue, maxValue, stepSize);
                InitProperty(whiteBalanceSlider, whiteBalanceCheckBox, _whiteBalancePropertyInfo);
            }
            catch (DirectShowCameraException)
            {
                DisableProperty(whiteBalanceLabel, whiteBalanceSlider, whiteBalanceCheckBox);
            }

            _isInitialized = true;
        }

        private void InitProperty(
            Slider slider,
            CheckBox checkBox,
            ImageQualityPropertyInfo propertyInfo)
        {
            slider.Minimum = propertyInfo.MinValue;
            slider.Maximum = propertyInfo.MaxValue;
            slider.Value = propertyInfo.CurrentValue;
            slider.IsEnabled = !propertyInfo.Auto;

            checkBox.IsChecked = propertyInfo.Auto;
        }

        private void DisableProperty(Label label, Slider slider, CheckBox checkBox)
        {
            label.IsEnabled = false;
            slider.IsEnabled = false;
            checkBox.IsEnabled = false;
        }


        #endregion


        #region Sliders

        /// <summary>
        /// Handles the ValueChanged event of backlightCompensationSlider object.
        /// </summary>
        private void backlightCompensationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetBacklightCompensationValue(backlightCompensationSlider.Value, false);
        }

        /// <summary>
        /// Handles the ValueChanged event of brightnessSlider object.
        /// </summary>
        private void brightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetBrightnessValue(brightnessSlider.Value, false);
        }

        /// <summary>
        /// Handles the ValueChanged event of colorEnableSlider object.
        /// </summary>
        private void colorEnableSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetColorEnableValue(colorEnableSlider.Value, false);
        }

        /// <summary>
        /// Handles the ValueChanged event of contrastSlider object.
        /// </summary>
        private void contrastSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetContrastValue(contrastSlider.Value, false);
        }

        /// <summary>
        /// Handles the ValueChanged event of gainSlider object.
        /// </summary>
        private void gainSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetGainValue(gainSlider.Value, false);
        }

        /// <summary>
        /// Handles the ValueChanged event of gammaSlider object.
        /// </summary>
        private void gammaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetGammaValue(gammaSlider.Value, false);
        }

        /// <summary>
        /// Handles the ValueChanged event of hueSlider object.
        /// </summary>
        private void hueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetHueValue(hueSlider.Value, false);
        }

        /// <summary>
        /// Handles the ValueChanged event of saturationSlider object.
        /// </summary>
        private void saturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetSaturationValue(saturationSlider.Value, false);
        }

        /// <summary>
        /// Handles the ValueChanged event of sharpnessSlider object.
        /// </summary>
        private void sharpnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetSharpnessValue(sharpnessSlider.Value, false);
        }

        /// <summary>
        /// Handles the ValueChanged event of whiteBalanceSlider object.
        /// </summary>
        private void whiteBalanceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetWhiteBalanceValue(whiteBalanceSlider.Value, false);
        }

        #endregion


        #region Check boxes

        /// <summary>
        /// Handles the CheckedChanged event of backlightCompensationCheckBox object.
        /// </summary>
        private void backlightCompensationCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetBacklightCompensationValue(backlightCompensationSlider.Value, backlightCompensationCheckBox.IsChecked.Value);
        }

        /// <summary>
        /// Handles the CheckedChanged event of brightnessCheckBox object.
        /// </summary>
        private void brightnessCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetBrightnessValue(brightnessSlider.Value, brightnessCheckBox.IsChecked.Value);
        }

        /// <summary>
        /// Handles the CheckedChanged event of colorEnableCheckBox object.
        /// </summary>
        private void colorEnableCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetColorEnableValue(colorEnableSlider.Value, colorEnableCheckBox.IsChecked.Value);
        }

        /// <summary>
        /// Handles the CheckedChanged event of contrastCheckBox object.
        /// </summary>
        private void contrastCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetContrastValue(contrastSlider.Value, contrastCheckBox.IsChecked.Value);
        }

        /// <summary>
        /// Handles the CheckedChanged event of gainCheckBox object.
        /// </summary>
        private void gainCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetGainValue(gainSlider.Value, gainCheckBox.IsChecked.Value);
        }

        /// <summary>
        /// Handles the CheckedChanged event of gammaCheckBox object.
        /// </summary>
        private void gammaCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetGammaValue(gammaSlider.Value, gammaCheckBox.IsChecked.Value);
        }

        /// <summary>
        /// Handles the CheckedChanged event of hueCheckBox object.
        /// </summary>
        private void hueCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetHueValue(hueSlider.Value, hueCheckBox.IsChecked.Value);
        }

        /// <summary>
        /// Handles the CheckedChanged event of saturationCheckBox object.
        /// </summary>
        private void saturationCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetSaturationValue(saturationSlider.Value, saturationCheckBox.IsChecked.Value);
        }

        /// <summary>
        /// Handles the CheckedChanged event of sharpnessCheckBox object.
        /// </summary>
        private void sharpnessCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetSharpnessValue(sharpnessSlider.Value, sharpnessCheckBox.IsChecked.Value);
        }

        /// <summary>
        /// Handles the CheckedChanged event of whiteBalanceCheckBox object.
        /// </summary>
        private void whiteBalanceCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetWhiteBalanceValue(whiteBalanceSlider.Value, whiteBalanceCheckBox.IsChecked.Value);
        }

        #endregion


        /// <summary>
        /// Resets the values of image quality properties.
        /// </summary>
        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            if (backlightCompensationLabel.IsEnabled)
                SetBacklightCompensationValue(_backlightCompensationPropertyInfo.DefaultValue, false);

            if (brightnessLabel.IsEnabled)
                SetBrightnessValue(_brightnessPropertyInfo.DefaultValue, false);

            if (colorEnableLabel.IsEnabled)
                SetColorEnableValue(_colorEnablePropertyInfo.DefaultValue, false);

            if (contrastLabel.IsEnabled)
                SetContrastValue(_contrastPropertyInfo.DefaultValue, false);

            if (gainLabel.IsEnabled)
                SetGainValue(_gainPropertyInfo.DefaultValue, false);

            if (gammaLabel.IsEnabled)
                SetGammaValue(_gammaPropertyInfo.DefaultValue, false);

            if (hueLabel.IsEnabled)
                SetHueValue(_huePropertyInfo.DefaultValue, false);

            if (saturationLabel.IsEnabled)
                SetSaturationValue(_saturationPropertyInfo.DefaultValue, false);

            if (sharpnessLabel.IsEnabled)
                SetSharpnessValue(_sharpnessPropertyInfo.DefaultValue, false);

            if (whiteBalanceLabel.IsEnabled)
                SetWhiteBalanceValue(_whiteBalancePropertyInfo.DefaultValue, false);
        }

        /// <summary>
        /// Restores the values of image quality properties.
        /// </summary>
        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (backlightCompensationLabel.IsEnabled)
                SetBacklightCompensationValue(_backlightCompensationPropertyInfo.CurrentValue, _backlightCompensationPropertyInfo.Auto);

            if (brightnessLabel.IsEnabled)
                SetBrightnessValue(_brightnessPropertyInfo.CurrentValue, _brightnessPropertyInfo.Auto);

            if (colorEnableLabel.IsEnabled)
                SetColorEnableValue(_colorEnablePropertyInfo.CurrentValue, _colorEnablePropertyInfo.Auto);

            if (contrastLabel.IsEnabled)
                SetContrastValue(_contrastPropertyInfo.CurrentValue, _contrastPropertyInfo.Auto);

            if (gainLabel.IsEnabled)
                SetGainValue(_gainPropertyInfo.CurrentValue, _gainPropertyInfo.Auto);

            if (gammaLabel.IsEnabled)
                SetGammaValue(_gammaPropertyInfo.CurrentValue, _gammaPropertyInfo.Auto);

            if (hueLabel.IsEnabled)
                SetHueValue(_huePropertyInfo.CurrentValue, _huePropertyInfo.Auto);

            if (saturationLabel.IsEnabled)
                SetSaturationValue(_saturationPropertyInfo.CurrentValue, _saturationPropertyInfo.Auto);

            if (sharpnessLabel.IsEnabled)
                SetSharpnessValue(_sharpnessPropertyInfo.CurrentValue, _sharpnessPropertyInfo.Auto);

            if (whiteBalanceLabel.IsEnabled)
                SetWhiteBalanceValue(_whiteBalancePropertyInfo.CurrentValue, _whiteBalancePropertyInfo.Auto);
        }


        #region Set property value

        /// <summary>
        /// Sets the backlight compensation value.
        /// </summary>
        private void SetBacklightCompensationValue(double value, bool autoMode)
        {
            if (!_isInitialized)
                return;

            try
            {
                _camera.ImageQuality.BacklightCompensation = new DirectShowImageQualityPropertyValue((int)value * _backlightCompensationPropertyInfo.StepSize, autoMode);

                if (backlightCompensationSlider.Value != value)
                    backlightCompensationSlider.Value = value;

                if (backlightCompensationCheckBox.IsChecked != autoMode)
                    backlightCompensationCheckBox.IsChecked = autoMode;

                backlightCompensationSlider.IsEnabled = !autoMode;
            }
            catch (DirectShowCameraException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Sets the brightness value.
        /// </summary>
        private void SetBrightnessValue(double value, bool autoMode)
        {
            if (!_isInitialized)
                return;

            try
            {
                _camera.ImageQuality.Brightness = new DirectShowImageQualityPropertyValue((int)value * _brightnessPropertyInfo.StepSize, autoMode);

                if (brightnessSlider.Value != value)
                    brightnessSlider.Value = value;

                if (brightnessCheckBox.IsChecked != autoMode)
                    brightnessCheckBox.IsChecked = autoMode;

                brightnessSlider.IsEnabled = !autoMode;
            }
            catch (DirectShowCameraException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Sets the color enable value.
        /// </summary>
        private void SetColorEnableValue(double value, bool autoMode)
        {
            if (!_isInitialized)
                return;

            try
            {
                _camera.ImageQuality.ColorEnable = new DirectShowImageQualityPropertyValue((int)value * _colorEnablePropertyInfo.StepSize, autoMode);

                if (colorEnableSlider.Value != value)
                    colorEnableSlider.Value = value;

                if (colorEnableCheckBox.IsChecked != autoMode)
                    colorEnableCheckBox.IsChecked = autoMode;

                colorEnableSlider.IsEnabled = !autoMode;
            }
            catch (DirectShowCameraException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Sets the contrast value.
        /// </summary>
        private void SetContrastValue(double value, bool autoMode)
        {
            if (!_isInitialized)
                return;

            try
            {
                _camera.ImageQuality.Contrast = new DirectShowImageQualityPropertyValue((int)value * _contrastPropertyInfo.StepSize, autoMode);

                if (contrastSlider.Value != value)
                    contrastSlider.Value = value;

                if (contrastCheckBox.IsChecked != autoMode)
                    contrastCheckBox.IsChecked = autoMode;

                contrastSlider.IsEnabled = !autoMode;
            }
            catch (DirectShowCameraException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Sets the gain value.
        /// </summary>
        private void SetGainValue(double value, bool autoMode)
        {
            if (!_isInitialized)
                return;

            try
            {
                _camera.ImageQuality.Gain = new DirectShowImageQualityPropertyValue((int)value * _gainPropertyInfo.StepSize, autoMode);

                if (gainSlider.Value != value)
                    gainSlider.Value = value;

                if (gainCheckBox.IsChecked != autoMode)
                    gainCheckBox.IsChecked = autoMode;

                gainSlider.IsEnabled = !autoMode;
            }
            catch (DirectShowCameraException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Sets the gamma value.
        /// </summary>
        private void SetGammaValue(double value, bool autoMode)
        {
            if (!_isInitialized)
                return;

            try
            {
                _camera.ImageQuality.Gamma = new DirectShowImageQualityPropertyValue((int)value * _gammaPropertyInfo.StepSize, autoMode);

                if (gammaSlider.Value != value)
                    gammaSlider.Value = value;

                if (gammaCheckBox.IsChecked != autoMode)
                    gammaCheckBox.IsChecked = autoMode;

                gammaSlider.IsEnabled = !autoMode;
            }
            catch (DirectShowCameraException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Sets the hue value.
        /// </summary>
        private void SetHueValue(double value, bool autoMode)
        {
            if (!_isInitialized)
                return;

            try
            {
                _camera.ImageQuality.Hue = new DirectShowImageQualityPropertyValue((int)value * _huePropertyInfo.StepSize, autoMode);

                if (hueSlider.Value != value)
                    hueSlider.Value = value;

                if (hueCheckBox.IsChecked != autoMode)
                    hueCheckBox.IsChecked = autoMode;

                hueSlider.IsEnabled = !autoMode;
            }
            catch (DirectShowCameraException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Sets the saturation value.
        /// </summary>
        private void SetSaturationValue(double value, bool autoMode)
        {
            if (!_isInitialized)
                return;

            try
            {
                _camera.ImageQuality.Saturation = new DirectShowImageQualityPropertyValue((int)value * _saturationPropertyInfo.StepSize, autoMode);

                if (saturationSlider.Value != value)
                    saturationSlider.Value = value;

                if (saturationCheckBox.IsChecked != autoMode)
                    saturationCheckBox.IsChecked = autoMode;

                saturationSlider.IsEnabled = !autoMode;
            }
            catch (DirectShowCameraException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Sets the sharpness value.
        /// </summary>
        private void SetSharpnessValue(double value, bool autoMode)
        {
            if (!_isInitialized)
                return;

            try
            {
                _camera.ImageQuality.Sharpness = new DirectShowImageQualityPropertyValue((int)value * _sharpnessPropertyInfo.StepSize, autoMode);

                if (sharpnessSlider.Value != value)
                    sharpnessSlider.Value = value;

                if (sharpnessCheckBox.IsChecked != autoMode)
                    sharpnessCheckBox.IsChecked = autoMode;

                sharpnessSlider.IsEnabled = !autoMode;
            }
            catch (DirectShowCameraException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Sets the white balance value.
        /// </summary>
        private void SetWhiteBalanceValue(double value, bool autoMode)
        {
            if (!_isInitialized)
                return;

            try
            {
                _camera.ImageQuality.WhiteBalance = new DirectShowImageQualityPropertyValue((int)value * _whiteBalancePropertyInfo.StepSize, autoMode);

                if (whiteBalanceSlider.Value != value)
                    whiteBalanceSlider.Value = value;

                if (whiteBalanceCheckBox.IsChecked != autoMode)
                    whiteBalanceCheckBox.IsChecked = autoMode;

                whiteBalanceSlider.IsEnabled = !autoMode;
            }
            catch (DirectShowCameraException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

    }
}
