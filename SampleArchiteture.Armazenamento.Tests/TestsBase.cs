﻿using Autofac;
using NUnit.Framework;

namespace SampleArchiteture.Armazenamento.Tests
{
    public abstract class TestsBase
    {
        protected IContainer Container;

        [SetUp]
        protected void Init()
        {
            Container = SetupTest.Container;

            OnInit();
        }

        protected abstract void OnInit();
    }
}