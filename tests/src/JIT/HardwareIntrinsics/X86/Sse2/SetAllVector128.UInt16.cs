// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
 * This file is auto-generated from a template file by the GenerateTests.csx  *
 * script in tests\src\JIT\HardwareIntrinsics\X86\Shared. In order to make    *
 * changes, please update the corresponding template and run according to the *
 * directions listed in the file.                                             *
 ******************************************************************************/

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace JIT.HardwareIntrinsics.X86
{
    public static partial class Program
    {
        private static void SetAllVector128UInt16()
        {
            var test = new ScalarSimdUnaryOpTest__SetAllVector128UInt16();

            if (test.IsSupported)
            {
                // Validates basic functionality works
                test.RunBasicScenario_UnsafeRead();

                // Validates calling via reflection works
                test.RunReflectionScenario_UnsafeRead();

                // Validates passing a static member works
                test.RunClsVarScenario();

                // Validates passing a local works
                test.RunLclVarScenario_UnsafeRead();

                // Validates passing the field of a local class works
                test.RunClassLclFldScenario();

                // Validates passing an instance member of a class works
                test.RunClassFldScenario();

                // Validates passing the field of a local struct works
                test.RunStructLclFldScenario();

                // Validates passing an instance member of a struct works
                test.RunStructFldScenario();
            }
            else
            {
                // Validates we throw on unsupported hardware
                test.RunUnsupportedScenario();
            }

            if (!test.Succeeded)
            {
                throw new Exception("One or more scenarios did not complete as expected.");
            }
        }
    }

    public sealed unsafe class ScalarSimdUnaryOpTest__SetAllVector128UInt16
    {
        private struct TestStruct
        {
            public UInt16 _fld;

            public static TestStruct Create()
            {
                var testStruct = new TestStruct();
                var random = new Random();

                testStruct._fld = (ushort)(random.Next(ushort.MinValue, ushort.MaxValue));
                return testStruct;
            }

            public void RunStructFldScenario(ScalarSimdUnaryOpTest__SetAllVector128UInt16 testClass)
            {
                var result = Sse2.SetAllVector128(_fld);

                Unsafe.Write(testClass._dataTable.outArrayPtr, result);
                testClass.ValidateResult(_fld, testClass._dataTable.outArrayPtr);
            }
        }

        private static readonly int LargestVectorSize = 16;

        private static readonly int RetElementCount = Unsafe.SizeOf<Vector128<UInt16>>() / sizeof(UInt16);

        private static UInt16 _data;

        private static UInt16 _clsVar;

        private UInt16 _fld;

        private ScalarSimdUnaryOpTest__DataTable<UInt16> _dataTable;

        static ScalarSimdUnaryOpTest__SetAllVector128UInt16()
        {
            var random = new Random();
            _clsVar = (ushort)(random.Next(ushort.MinValue, ushort.MaxValue));
        }

        public ScalarSimdUnaryOpTest__SetAllVector128UInt16()
        {
            Succeeded = true;

            var random = new Random();
            _fld = (ushort)(random.Next(ushort.MinValue, ushort.MaxValue));
            _data = (ushort)(random.Next(ushort.MinValue, ushort.MaxValue));
            _dataTable = new ScalarSimdUnaryOpTest__DataTable<UInt16>(new UInt16[RetElementCount], LargestVectorSize);
        }

        public bool IsSupported => Sse2.IsSupported && (Environment.Is64BitProcess || ((typeof(UInt16) != typeof(long)) && (typeof(UInt16) != typeof(ulong))));

        public bool Succeeded { get; set; }

        public void RunBasicScenario_UnsafeRead()
        {
            var result = Sse2.SetAllVector128(
                Unsafe.ReadUnaligned<UInt16>(ref Unsafe.As<UInt16, byte>(ref _data))
            );

            Unsafe.Write(_dataTable.outArrayPtr, result);
            ValidateResult(_data, _dataTable.outArrayPtr);
        }

        public void RunReflectionScenario_UnsafeRead()
        {
            var result = typeof(Sse2).GetMethod(nameof(Sse2.SetAllVector128), new Type[] { typeof(UInt16) })
                                     .Invoke(null, new object[] {
                                        Unsafe.ReadUnaligned<UInt16>(ref Unsafe.As<UInt16, byte>(ref _data))
                                     });

            Unsafe.Write(_dataTable.outArrayPtr, (Vector128<UInt16>)(result));
            ValidateResult(_data, _dataTable.outArrayPtr);
        }

        public void RunClsVarScenario()
        {
            var result = Sse2.SetAllVector128(
                _clsVar
            );

            Unsafe.Write(_dataTable.outArrayPtr, result);
            ValidateResult(_clsVar, _dataTable.outArrayPtr);
        }

        public void RunLclVarScenario_UnsafeRead()
        {
            var data = Unsafe.ReadUnaligned<UInt16>(ref Unsafe.As<UInt16, byte>(ref _data));
            var result = Sse2.SetAllVector128(data);

            Unsafe.Write(_dataTable.outArrayPtr, result);
            ValidateResult(data, _dataTable.outArrayPtr);
        }

        public void RunClassLclFldScenario()
        {
            var test = new ScalarSimdUnaryOpTest__SetAllVector128UInt16();
            var result = Sse2.SetAllVector128(test._fld);

            Unsafe.Write(_dataTable.outArrayPtr, result);
            ValidateResult(test._fld, _dataTable.outArrayPtr);
        }

        public void RunClassFldScenario()
        {
            var result = Sse2.SetAllVector128(_fld);

            Unsafe.Write(_dataTable.outArrayPtr, result);
            ValidateResult(_fld, _dataTable.outArrayPtr);
        }

        public void RunStructLclFldScenario()
        {
            var test = TestStruct.Create();
            var result = Sse2.SetAllVector128(test._fld);

            Unsafe.Write(_dataTable.outArrayPtr, result);
            ValidateResult(test._fld, _dataTable.outArrayPtr);
        }

        public void RunStructFldScenario()
        {
            var test = TestStruct.Create();
            test.RunStructFldScenario(this);
        }

        public void RunUnsupportedScenario()
        {
            Succeeded = false;

            try
            {
                RunBasicScenario_UnsafeRead();
            }
            catch (PlatformNotSupportedException)
            {
                Succeeded = true;
            }
        }

        private void ValidateResult(UInt16 firstOp, void* result, [CallerMemberName] string method = "")
        {
            UInt16[] outArray = new UInt16[RetElementCount];

            Unsafe.CopyBlockUnaligned(ref Unsafe.As<UInt16, byte>(ref outArray[0]), ref Unsafe.AsRef<byte>(result), (uint)Unsafe.SizeOf<Vector128<UInt16>>());

            ValidateResult(firstOp, outArray, method);
        }

        private void ValidateResult(UInt16 firstOp, UInt16[] result, [CallerMemberName] string method = "")
        {
            if (result[0] != firstOp)
            {
                Succeeded = false;
            }
            else
            {
                for (var i = 1; i < RetElementCount; i++)
                {
                    if (result[i] != firstOp)
                    {
                        Succeeded = false;
                        break;
                    }
                }
            }

            if (!Succeeded)
            {
                Console.WriteLine($"{nameof(Sse2)}.{nameof(Sse2.SetAllVector128)}<UInt16>(Vector128<UInt16>): {method} failed:");
                Console.WriteLine($"  firstOp: ({string.Join(", ", firstOp)})");
                Console.WriteLine($"   result: ({string.Join(", ", result)})");
                Console.WriteLine();
            }
        }
    }
}
