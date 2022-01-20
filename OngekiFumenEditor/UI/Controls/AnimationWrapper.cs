﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace OngekiFumenEditor.UI.Controls
{
    public class AnimationWrapper
    {
        private AnimationTimeline animation = default;
        private FrameworkElement frameworkElement;

        private AnimationClock clock;
        private readonly DependencyProperty dp;

        private ClockController Controller => clock.Controller;

        public AnimationWrapper(AnimationTimeline animation, FrameworkElement frameworkElement, DependencyProperty dp)
        {
            this.animation = animation;
            this.frameworkElement = frameworkElement;
            this.dp = dp;
        }

        public virtual void Start()
        {
            clock = animation.CreateClock();
            frameworkElement.ApplyAnimationClock(dp, clock);
            Controller.Begin();
        }

        public virtual void Resume()
        {
            Controller.Resume();
        }

        public virtual void Stop()
        {
            Controller.Stop();
        }

        public virtual void Pause()
        {
            Controller.Pause();
        }

        public void JumpAndPause(TimeSpan offset, TimeSeekOrigin origin = TimeSeekOrigin.BeginTime)
        {
            Pause();
            Jump(offset, origin);
        }

        public virtual void Jump(TimeSpan offset, TimeSeekOrigin origin = TimeSeekOrigin.BeginTime)
        {
            Controller.Seek(offset, origin);
        }
    }
}
