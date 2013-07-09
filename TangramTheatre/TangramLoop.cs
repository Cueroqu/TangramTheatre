using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace TangramTheatre
{
    public class TangramLoop : IDisposable
    {
        private DateTime lastUpdateTime = DateTime.MinValue;

        public delegate void UpdateDelegate(TimeSpan elapsedTime);

        public event UpdateDelegate Update;

        public delegate void IsRunningChangedDelegate(bool isEnabled);

        public event IsRunningChangedDelegate IsRunningChanged;

        private void BeforeRender(object sender, EventArgs e)
        {
            if (Update != null)
            {
                TimeSpan elapsedTime = DateTime.Now - lastUpdateTime;
                lastUpdateTime = DateTime.Now;
                Update(elapsedTime);
            }
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                if (value) Start();
                else Stop();
            }
        }

        public void Start()
        {
            lastUpdateTime = DateTime.Now;
            if (_isRunning) return;
            _isRunning = true;
            CompositionTarget.Rendering += BeforeRender;
            if (IsRunningChanged != null)
                IsRunningChanged(true);
        }

        public void Stop()
        {
            if (!_isRunning) return;
            _isRunning = false;
            CompositionTarget.Rendering -= BeforeRender;
            if (IsRunningChanged != null)
                IsRunningChanged(false);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
