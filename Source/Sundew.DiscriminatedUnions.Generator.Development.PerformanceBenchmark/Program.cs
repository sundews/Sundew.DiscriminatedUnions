// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

#if DEBUG
BenchmarkRunner.Run(typeof(Program).Assembly, new DebugInProcessConfig());
#else
BenchmarkRunner.Run(typeof(Program).Assembly, ManualConfig.Create(DefaultConfig.Instance));
#endif
