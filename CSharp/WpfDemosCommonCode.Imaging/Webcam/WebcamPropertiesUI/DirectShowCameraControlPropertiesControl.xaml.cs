using System.Windows;
using System.Windows.Controls;

using Vintasoft.Imaging.Media;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A control that allows to change the camera control properties.
    /// </summary>
    public partial class DirectShowCameraControlPropertiesControl : UserControl
    {

        #region Structs

        /// <summary>
        /// Stores information about values of the camera control property.
        /// </summary>
        class CameraControlPropertyInfo
        {
            internal int CurrentValue;
            internal int DefaultValue;
            internal int MinValue;
            internal int MaxValue;
            internal int StepSize;
            internal bool Auto;


            internal CameraControlPropertyInfo(
                DirectShowCameraControlPropertyValue currentValue,
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

        CameraControlPropertyInfo _exposurePropertyInfo;
        CameraControlPropertyInfo _focusPropertyInfo;
        CameraControlPropertyInfo _irisPropertyInfo;
        CameraControlPropertyInfo _panPropertyInfo;
        CameraControlPropertyInfo _rollPropertyInfo;
        CameraControlPropertyInfo _tiltPropertyInfo;
        CameraControlPropertyInfo _zoomPropertyInfo;

        bool _isInitialized;

        #endregion



        #region Constructors

        public DirectShowCameraControlPropertiesControl()
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
                cameraControlPropertiesGroupBox.IsEnabled = false;
                return;
            }

            DirectShowCameraControlPropertyValue currentValue;
            int defaultValue, minValue, maxValue, stepSize;

            // exposure
            try
            {
                _camera.CameraControl.GetSupportedExposureValues(out minValue, out maxValue, out stepSize, out defaultValue);
                currentValue = _camera.CameraControl.Exposure;

                _exposurePropertyInfo = new CameraControlPropertyInfo(currentValue, defaultValue, minValue, maxValue, stepSize);
                InitProperty(exposureSlider, exposureCheckBox, _exposurePropertyInfo);
            }
            catch (DirectShowCameraException)
            {
                DisableProperty(exposureLabel, exposureSlider, exposureCheckBox);
            }

            // focus
            try
            {
                _camera.CameraControl.GetSupportedFocusValues(out minValue, out maxValue, out stepSize, out defaultValue);
                currentValue = _camera.CameraControl.Focus;

                _focusPropertyInfo = new CameraControlPropertyInfo(currentValue, defaultValue, minValue, maxValue, stepSize);
                InitProperty(focusSlider, focusCheckBox, _focusPropertyInfo);
            }
            catch (DirectShowCameraException)
            {
                DisableProperty(focusLabel, focusSlider, focusCheckBox);
            }

            // iris
            try
            {
                _camera.CameraControl.GetSupportedIrisValues(out minValue, out maxValue, out stepSize, out defaultValue);
                currentValue = _camera.CameraControl.Iris;

                _irisPropertyInfo = new CameraControlPropertyInfo(currentValue, defaultValue, minValue, maxValue, stepSize);
                InitProperty(irisSlider, irisCheckBox, _irisPropertyInfo);
            }
            catch (DirectShowCameraException)
            {
                DisableProperty(irisLabel, irisSlider, irisCheckBox);
            }

            // pan
            try
            {
                _camera.CameraControl.GetSupportedPanValues(out minValue, out maxValue, out stepSize, out defaultValue);
                currentValue = _camera.CameraControl.Pan;

                _panPropertyInfo = new CameraControlPropertyInfo(currentValue, defaultValue, minValue, maxValue, stepSize);
                InitProperty(panSlider, panCheckBox, _panPropertyInfo);
            }
            catch (DirectShowCameraException)
            {
                DisableProperty(panLabel, panSlider, panCheckBox);
            }

            // roll
            try
            {
                _camera.CameraControl.GetSupportedRollValues(out minValue, out maxValue, out stepSize, out defaultValue);
                currentValue = _camera.CameraControl.Roll;

                _rollPropertyInfo = new CameraControlPropertyInfo(currentValue, defaultValue, minValue, maxValue, stepSize);
                InitProperty(rollSlider, rollCheckBox, _rollPropertyInfo);
            }
            catch (DirectShowCameraException)
            {
                DisableProperty(rollLabel, rollSlider, rollCheckBox);
            }

            // tilt
            try
            {
                _camera.CameraControl.GetSupportedTiltValues(out minValue, out maxValue, out stepSize, out defaultValue);
                currentValue = _camera.CameraControl.Tilt;

                _tiltPropertyInfo = new CameraControlPropertyInfo(currentValue, defaultValue, minValue, maxValue, stepSize);
                InitProperty(tiltSlider, tiltCheckBox, _tiltPropertyInfo);
            }
            catch (DirectShowCameraException)
            {
                DisableProperty(tiltLabel, tiltSlider, tiltCheckBox);
            }

            // zoom
            try
            {
                _camera.CameraControl.GetSupportedZoomValues(out minValue, out maxValue, out stepSize, out defaultValue);
                currentValue = _camera.CameraControl.Zoom;

                _zoomPropertyInfo = new CameraControlPropertyInfo(currentValue, defaultValue, minValue, maxValue, stepSize);
                InitProperty(zoomSlider, zoomCheckBox, _zoomPropertyInfo);
            }
            catch (DirectShowCameraException)
            {
                DisableProperty(zoomLabel, zoomSlider, zoomCheckBox);
            }

            _isInitialized = true;
        }


        private void InitProperty(
            Slider slider,
            CheckBox checkBox,
            CameraControlPropertyInfo propertyInfo)
        {
            slider.Minimum = propertyInfo.MinValue;
            slider.Maximum = propertyInfo.MaxValue;
            slider.Value = propertyInfo.CurrentValue;
            slider.IsEnabled = !propertyInfo.Auto;

            checkBox.IsChecked = propertyInfo.Auto;
        }

        private void DisableProperty(Label label, Slider Slider, CheckBox checkBox)
        {
            label.IsEnabled = false;
            Slider.IsEnabled = false;
            checkBox.IsEnabled = false;
        }


        #region Sliders

        /// <summary>
        /// Handles the ValueChanged event of exposureSlider object.
        /// </summary>
        private void exposureSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetExposureValue(exposureSlider.Value, false);
        }

        /// <summary>
        /// Handles the ValueChanged event of focusSlider object.
        /// </summary>
        private void focusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetFocusValue(focusSlider.Value, false);
        }

        /// <summary>
        /// Handles the ValueChanged event of irisSlider object.
        /// </summary>
        private void irisSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetIrisValue(irisSlider.Value, false);
        }

        /// <summary>
        /// Handles the ValueChanged event of panSlider object.
        /// </summary>
        private void panSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetPanValue(panSlider.Value, false);
        }

        /// <summary>
        /// Handles the ValueChanged event of rollSlider object.
        /// </summary>
        private void rollSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetRollValue(rollSlider.Value, false);
        }

        /// <summary>
        /// Handles the ValueChanged event of tiltSlider object.
        /// </summary>
        private void tiltSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetTiltValue(tiltSlider.Value, false);
        }

        /// <summary>
        /// Handles the ValueChanged event of zoomSlider object.
        /// </summary>
        private void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetZoomValue(zoomSlider.Value, false);
        }

        #endregion


        #region Check boxes

        /// <summary>
        /// Handles the CheckedChanged event of exposureCheckBox object.
        /// </summary>
        private void exposureCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetExposureValue(exposureSlider.Value, exposureCheckBox.IsChecked.Value);
        }

        /// <summary>
        /// Handles the CheckedChanged event of focusCheckBox object.
        /// </summary>
        private void focusCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetFocusValue(focusSlider.Value, focusCheckBox.IsChecked.Value);
        }

        /// <summary>
        /// Handles the CheckedChanged event of irisCheckBox object.
        /// </summary>
        private void irisCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetIrisValue(irisSlider.Value, irisCheckBox.IsChecked.Value);
        }

        /// <summary>
        /// Handles the CheckedChanged event of panCheckBox object.
        /// </summary>
        private void panCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetPanValue(panSlider.Value, panCheckBox.IsChecked.Value);
        }

        /// <summary>
        /// Handles the CheckedChanged event of rollCheckBox object.
        /// </summary>
        private void rollCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetRollValue(rollSlider.Value, rollCheckBox.IsChecked.Value);
        }

        /// <summary>
        /// Handles the CheckedChanged event of tiltCheckBox object.
        /// </summary>
        private void tiltCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetTiltValue(tiltSlider.Value, tiltCheckBox.IsChecked.Value);
        }

        /// <summary>
        /// Handles the CheckedChanged event of zoomCheckBox object.
        /// </summary>
        private void zoomCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetZoomValue(zoomSlider.Value, zoomCheckBox.IsChecked.Value);
        }

        #endregion


        /// <summary>
        /// Resets the values of camera control properties.
        /// </summary>
        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            if (exposureLabel.IsEnabled)
                SetExposureValue(_exposurePropertyInfo.DefaultValue, false);

            if (focusLabel.IsEnabled)
                SetExposureValue(_focusPropertyInfo.DefaultValue, false);

            if (irisLabel.IsEnabled)
                SetIrisValue(_irisPropertyInfo.DefaultValue, false);

            if (panLabel.IsEnabled)
                SetPanValue(_panPropertyInfo.DefaultValue, false);

            if (rollLabel.IsEnabled)
                SetRollValue(_rollPropertyInfo.DefaultValue, false);

            if (tiltLabel.IsEnabled)
                SetTiltValue(_tiltPropertyInfo.DefaultValue, false);

            if (zoomLabel.IsEnabled)
                SetZoomValue(_zoomPropertyInfo.DefaultValue, false);
        }

        /// <summary>
        /// Restores the values of camera control properties.
        /// </summary>
        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (exposureLabel.IsEnabled)
                SetExposureValue(_exposurePropertyInfo.CurrentValue, _exposurePropertyInfo.Auto);

            if (focusLabel.IsEnabled)
                SetExposureValue(_focusPropertyInfo.CurrentValue, _focusPropertyInfo.Auto);

            if (irisLabel.IsEnabled)
                SetIrisValue(_irisPropertyInfo.CurrentValue, _irisPropertyInfo.Auto);

            if (panLabel.IsEnabled)
                SetPanValue(_panPropertyInfo.CurrentValue, _panPropertyInfo.Auto);

            if (rollLabel.IsEnabled)
                SetRollValue(_rollPropertyInfo.CurrentValue, _rollPropertyInfo.Auto);

            if (tiltLabel.IsEnabled)
                SetTiltValue(_tiltPropertyInfo.CurrentValue, _tiltPropertyInfo.Auto);

            if (zoomLabel.IsEnabled)
                SetZoomValue(_zoomPropertyInfo.CurrentValue, _zoomPropertyInfo.Auto);
        }


        #region Set property value

        /// <summary>
        /// Sets the exposure value.
        /// </summary>
        private void SetExposureValue(double value, bool autoMode)
        {
            if (!_isInitialized)
                return;

            try
            {
                _camera.CameraControl.Exposure = new DirectShowCameraControlPropertyValue((int)value * _exposurePropertyInfo.StepSize, autoMode);

                if (exposureSlider.Value != value)
                    exposureSlider.Value = value;

                if (exposureCheckBox.IsChecked != autoMode)
                    exposureCheckBox.IsChecked = autoMode;

                exposureSlider.IsEnabled = !autoMode;
            }
            catch (DirectShowCameraException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Sets the focus value.
        /// </summary>
        private void SetFocusValue(double value, bool autoMode)
        {
            if (!_isInitialized)
                return;

            try
            {
                _camera.CameraControl.Focus = new DirectShowCameraControlPropertyValue((int)value * _focusPropertyInfo.StepSize, autoMode);

                if (focusSlider.Value != value)
                    focusSlider.Value = value;

                if (focusCheckBox.IsChecked != autoMode)
                    focusCheckBox.IsChecked = autoMode;

                focusSlider.IsEnabled = !autoMode;
            }
            catch (DirectShowCameraException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Sets the iris value.
        /// </summary>
        private void SetIrisValue(double value, bool autoMode)
        {
            if (!_isInitialized)
                return;

            try
            {
                _camera.CameraControl.Iris = new DirectShowCameraControlPropertyValue((int)value * _irisPropertyInfo.StepSize, autoMode);

                if (irisSlider.Value != value)
                    irisSlider.Value = value;

                if (irisCheckBox.IsChecked != autoMode)
                    irisCheckBox.IsChecked = autoMode;

                irisSlider.IsEnabled = !autoMode;
            }
            catch (DirectShowCameraException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Sets the pan value.
        /// </summary>
        private void SetPanValue(double value, bool autoMode)
        {
            if (!_isInitialized)
                return;

            try
            {
                _camera.CameraControl.Pan = new DirectShowCameraControlPropertyValue((int)value * _panPropertyInfo.StepSize, autoMode);

                if (panSlider.Value != value)
                    panSlider.Value = value;

                if (panCheckBox.IsChecked != autoMode)
                    panCheckBox.IsChecked = autoMode;

                panSlider.IsEnabled = !autoMode;
            }
            catch (DirectShowCameraException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Sets the roll value.
        /// </summary>
        private void SetRollValue(double value, bool autoMode)
        {
            if (!_isInitialized)
                return;

            try
            {
                _camera.CameraControl.Roll = new DirectShowCameraControlPropertyValue((int)value * _rollPropertyInfo.StepSize, autoMode);

                if (rollSlider.Value != value)
                    rollSlider.Value = value;

                if (rollCheckBox.IsChecked != autoMode)
                    rollCheckBox.IsChecked = autoMode;

                rollSlider.IsEnabled = !autoMode;
            }
            catch (DirectShowCameraException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Sets the tilt value.
        /// </summary>
        private void SetTiltValue(double value, bool autoMode)
        {
            if (!_isInitialized)
                return;

            try
            {
                _camera.CameraControl.Tilt = new DirectShowCameraControlPropertyValue((int)value * _tiltPropertyInfo.StepSize, autoMode);

                if (tiltSlider.Value != value)
                    tiltSlider.Value = value;

                if (tiltCheckBox.IsChecked != autoMode)
                    tiltCheckBox.IsChecked = autoMode;

                tiltSlider.IsEnabled = !autoMode;
            }
            catch (DirectShowCameraException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Sets the zoom value.
        /// </summary>
        private void SetZoomValue(double value, bool autoMode)
        {
            if (!_isInitialized)
                return;

            try
            {
                _camera.CameraControl.Zoom = new DirectShowCameraControlPropertyValue((int)value * _zoomPropertyInfo.StepSize, autoMode);

                if (zoomSlider.Value != value)
                    zoomSlider.Value = value;

                if (zoomCheckBox.IsChecked != autoMode)
                    zoomCheckBox.IsChecked = autoMode;

                zoomSlider.IsEnabled = !autoMode;
            }
            catch (DirectShowCameraException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion


        #endregion

    }
}
