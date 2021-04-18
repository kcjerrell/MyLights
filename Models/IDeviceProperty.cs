﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Models
{
    public interface IDeviceProperty<T> where T : IEquatable<T>
    {
        public T Value { get; }
        public Task Set(T value);
        public Task Update();

        public event PropertyChangedEventHandler Updated;
    }
}
