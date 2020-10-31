﻿using System;
using System.Reflection;
using AutoFixture;
using AutoFixture.NUnit3;
using Moq;
using ZptSharp.Dom;

namespace ZptSharp.Autofixture
{
    public class StubDomAttribute : CustomizeAttribute
    {
        public override ICustomization GetCustomization(ParameterInfo parameter)
            => new StubDomCustomization();

        public class StubDomCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<IDocument>(x =>
                {
                    return x.FromFactory(() =>
                    {
                        var mock = new Mock<StubDocument>() { CallBase = true };
                        return mock.Object;
                    });
                });

                fixture.Customize<IElement>(x =>
                {
                    return x
                        .FromFactory((IDocument doc) =>
                        {
                            var mock = new Mock<StubElement>(doc) { CallBase = true };
                            return mock.Object;
                        })
                        .OmitAutoProperties();
                });

                fixture.Customize<IAttribute>(x =>
                {
                    return x.FromFactory((string name, string value) =>
                    {
                        var mock = new Mock<StubAttribute>(Mock.Of<IElement>(), name, value) { CallBase = true };
                        return mock.Object;
                    });
                });
            }
        }
    }
}
