using System;
using Moq.AutoMock;

namespace PFire.Tests
{
    public abstract class BaseTest
    {
        // ReSharper disable once InconsistentNaming
        protected readonly AutoMocker _autoMoqer;

        // ReSharper disable once InconsistentNaming
        protected readonly Random _random;

        protected BaseTest()
        {
            _autoMoqer = new AutoMocker();
            _random = new Random();
        }
    }
}