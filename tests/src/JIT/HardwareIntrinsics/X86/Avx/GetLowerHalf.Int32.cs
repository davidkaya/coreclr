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
        private static void GetLowerHalfInt32()
        {
            var test = new GenericUnaryOpTest__GetLowerHalfInt32();

            if (test.IsSupported)
            {
                // Validates basic functionality works, using Unsafe.Read
                test.RunBasicScenario_UnsafeRead();

                if (Avx.IsSupported)
                {
                    // Validates basic functionality works, using Load
                    test.RunBasicScenario_Load();

                    // Validates basic functionality works, using LoadAligned
                    test.RunBasicScenario_LoadAligned();
                }

                // Validates calling via reflection works, using Unsafe.Read
                test.RunReflectionScenario_UnsafeRead();

                if (Avx.IsSupported)
                {
                    // Validates calling via reflection works, using Load
                    test.RunReflectionScenario_Load();

                    // Validates calling via reflection works, using LoadAligned
                    test.RunReflectionScenario_LoadAligned();
                }

                // Validates passing a static member works
                test.RunClsVarScenario();

                // Validates passing a local works, using Unsafe.Read
                test.RunLclVarScenario_UnsafeRead();

                if (Avx.IsSupported)
                {
                    // Validates passing a local works, using Load
                    test.RunLclVarScenario_Load();

                    // Validates passing a local works, using LoadAligned
                    test.RunLclVarScenario_LoadAligned();
                }

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

    public sealed unsafe class GenericUnaryOpTest__GetLowerHalfInt32
    {
        private struct TestStruct
        {
            public Vector256<Int32> _fld;

            public static TestStruct Create()
            {
                var testStruct = new TestStruct();
                var random = new Random();

                for (var i = 0; i < Op1ElementCount; i++) { _data[i] = (int)(random.Next(int.MinValue, int.MaxValue)); }
                Unsafe.CopyBlockUnaligned(ref Unsafe.As<Vector256<Int32>, byte>(ref testStruct._fld), ref Unsafe.As<Int32, byte>(ref _data[0]), (uint)Unsafe.SizeOf<Vector256<Int32>>());

                return testStruct;
            }

            public void RunStructFldScenario(GenericUnaryOpTest__GetLowerHalfInt32 testClass)
            {
                var result = Avx.GetLowerHalf<Int32>(_fld);

                Unsafe.Write(testClass._dataTable.outArrayPtr, result);
                testClass.ValidateResult(_fld, testClass._dataTable.outArrayPtr);
            }
        }

        private static readonly int LargestVectorSize = 32;

        private static readonly int Op1ElementCount = Unsafe.SizeOf<Vector256<Int32>>() / sizeof(Int32);
        private static readonly int RetElementCount = Unsafe.SizeOf<Vector128<Int32>>() / sizeof(Int32);

        private static Int32[] _data = new Int32[Op1ElementCount];

        private static Vector256<Int32> _clsVar;

        private Vector256<Int32> _fld;

        private SimpleUnaryOpTest__DataTable<Int32, Int32> _dataTable;

        static GenericUnaryOpTest__GetLowerHalfInt32()
        {
            var random = new Random();

            for (var i = 0; i < Op1ElementCount; i++) { _data[i] = (int)(random.Next(int.MinValue, int.MaxValue)); }
            Unsafe.CopyBlockUnaligned(ref Unsafe.As<Vector256<Int32>, byte>(ref _clsVar), ref Unsafe.As<Int32, byte>(ref _data[0]), (uint)Unsafe.SizeOf<Vector256<Int32>>());
        }

        public GenericUnaryOpTest__GetLowerHalfInt32()
        {
            Succeeded = true;

            var random = new Random();

            for (var i = 0; i < Op1ElementCount; i++) { _data[i] = (int)(random.Next(int.MinValue, int.MaxValue)); }
            Unsafe.CopyBlockUnaligned(ref Unsafe.As<Vector256<Int32>, byte>(ref _fld), ref Unsafe.As<Int32, byte>(ref _data[0]), (uint)Unsafe.SizeOf<Vector256<Int32>>());

            for (var i = 0; i < Op1ElementCount; i++) { _data[i] = (int)(random.Next(int.MinValue, int.MaxValue)); }
            _dataTable = new SimpleUnaryOpTest__DataTable<Int32, Int32>(_data, new Int32[RetElementCount], LargestVectorSize);
        }

        public bool IsSupported => Avx.IsSupported;

        public bool Succeeded { get; set; }

        public void RunBasicScenario_UnsafeRead()
        {
            var result = Avx.GetLowerHalf<Int32>(
                Unsafe.Read<Vector256<Int32>>(_dataTable.inArrayPtr)
            );

            Unsafe.Write(_dataTable.outArrayPtr, result);
            ValidateResult(_dataTable.inArrayPtr, _dataTable.outArrayPtr);
        }

        public void RunBasicScenario_Load()
        {
            var result = Avx.GetLowerHalf<Int32>(
                Avx.LoadVector256((Int32*)(_dataTable.inArrayPtr))
            );

            Unsafe.Write(_dataTable.outArrayPtr, result);
            ValidateResult(_dataTable.inArrayPtr, _dataTable.outArrayPtr);
        }

        public void RunBasicScenario_LoadAligned()
        {
            var result = Avx.GetLowerHalf<Int32>(
                Avx.LoadAlignedVector256((Int32*)(_dataTable.inArrayPtr))
            );

            Unsafe.Write(_dataTable.outArrayPtr, result);
            ValidateResult(_dataTable.inArrayPtr, _dataTable.outArrayPtr);
        }

        public void RunReflectionScenario_UnsafeRead()
        {
            var result = typeof(Avx).GetMethod(nameof(Avx.GetLowerHalf))
                                     .MakeGenericMethod( new Type[] { typeof(Int32) })
                                     .Invoke(null, new object[] {
                                        Unsafe.Read<Vector256<Int32>>(_dataTable.inArrayPtr)
                                     });

            Unsafe.Write(_dataTable.outArrayPtr, (Vector128<Int32>)(result));
            ValidateResult(_dataTable.inArrayPtr, _dataTable.outArrayPtr);
        }

        public void RunReflectionScenario_Load()
        {
            var result = typeof(Avx).GetMethod(nameof(Avx.GetLowerHalf))
                                     .MakeGenericMethod( new Type[] { typeof(Int32) })
                                     .Invoke(null, new object[] {
                                        Avx.LoadVector256((Int32*)(_dataTable.inArrayPtr))
                                     });

            Unsafe.Write(_dataTable.outArrayPtr, (Vector128<Int32>)(result));
            ValidateResult(_dataTable.inArrayPtr, _dataTable.outArrayPtr);
        }

        public void RunReflectionScenario_LoadAligned()
        {
            var result = typeof(Avx).GetMethod(nameof(Avx.GetLowerHalf))
                                     .MakeGenericMethod( new Type[] { typeof(Int32) })
                                     .Invoke(null, new object[] {
                                        Avx.LoadAlignedVector256((Int32*)(_dataTable.inArrayPtr))
                                     });

            Unsafe.Write(_dataTable.outArrayPtr, (Vector128<Int32>)(result));
            ValidateResult(_dataTable.inArrayPtr, _dataTable.outArrayPtr);
        }

        public void RunClsVarScenario()
        {
            var result = Avx.GetLowerHalf<Int32>(
                _clsVar
            );

            Unsafe.Write(_dataTable.outArrayPtr, result);
            ValidateResult(_clsVar, _dataTable.outArrayPtr);
        }

        public void RunLclVarScenario_UnsafeRead()
        {
            var firstOp = Unsafe.Read<Vector256<Int32>>(_dataTable.inArrayPtr);
            var result = Avx.GetLowerHalf<Int32>(firstOp);

            Unsafe.Write(_dataTable.outArrayPtr, result);
            ValidateResult(firstOp, _dataTable.outArrayPtr);
        }

        public void RunLclVarScenario_Load()
        {
            var firstOp = Avx.LoadVector256((Int32*)(_dataTable.inArrayPtr));
            var result = Avx.GetLowerHalf<Int32>(firstOp);

            Unsafe.Write(_dataTable.outArrayPtr, result);
            ValidateResult(firstOp, _dataTable.outArrayPtr);
        }

        public void RunLclVarScenario_LoadAligned()
        {
            var firstOp = Avx.LoadAlignedVector256((Int32*)(_dataTable.inArrayPtr));
            var result = Avx.GetLowerHalf<Int32>(firstOp);

            Unsafe.Write(_dataTable.outArrayPtr, result);
            ValidateResult(firstOp, _dataTable.outArrayPtr);
        }

        public void RunClassLclFldScenario()
        {
            var test = new GenericUnaryOpTest__GetLowerHalfInt32();
            var result = Avx.GetLowerHalf<Int32>(test._fld);

            Unsafe.Write(_dataTable.outArrayPtr, result);
            ValidateResult(test._fld, _dataTable.outArrayPtr);
        }

        public void RunClassFldScenario()
        {
            var result = Avx.GetLowerHalf<Int32>(_fld);

            Unsafe.Write(_dataTable.outArrayPtr, result);
            ValidateResult(_fld, _dataTable.outArrayPtr);
        }

        public void RunStructLclFldScenario()
        {
            var test = TestStruct.Create();
            var result = Avx.GetLowerHalf(test._fld);

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

        private void ValidateResult(Vector256<Int32> firstOp, void* result, [CallerMemberName] string method = "")
        {
            Int32[] inArray = new Int32[Op1ElementCount];
            Int32[] outArray = new Int32[RetElementCount];

            Unsafe.WriteUnaligned(ref Unsafe.As<Int32, byte>(ref inArray[0]), firstOp);
            Unsafe.CopyBlockUnaligned(ref Unsafe.As<Int32, byte>(ref outArray[0]), ref Unsafe.AsRef<byte>(result), (uint)Unsafe.SizeOf<Vector128<Int32>>());

            ValidateResult(inArray, outArray, method);
        }

        private void ValidateResult(void* firstOp, void* result, [CallerMemberName] string method = "")
        {
            Int32[] inArray = new Int32[Op1ElementCount];
            Int32[] outArray = new Int32[RetElementCount];

            Unsafe.CopyBlockUnaligned(ref Unsafe.As<Int32, byte>(ref inArray[0]), ref Unsafe.AsRef<byte>(firstOp), (uint)Unsafe.SizeOf<Vector256<Int32>>());
            Unsafe.CopyBlockUnaligned(ref Unsafe.As<Int32, byte>(ref outArray[0]), ref Unsafe.AsRef<byte>(result), (uint)Unsafe.SizeOf<Vector128<Int32>>());

            ValidateResult(inArray, outArray, method);
        }

        private void ValidateResult(Int32[] firstOp, Int32[] result, [CallerMemberName] string method = "")
        {
            if (firstOp[0] != result[0])
            {
                Succeeded = false;
            }
            else
            {
                for (var i = 1; i < RetElementCount; i++)
                {
                    if (result[i] != ((i < (Op1ElementCount / 2)) ? firstOp[i] : 0))
                    {
                        Succeeded = false;
                        break;
                    }
                }
            }

            if (!Succeeded)
            {
                Console.WriteLine($"{nameof(Avx)}.{nameof(Avx.GetLowerHalf)}<Int32>(Vector256<Int32>): {method} failed:");
                Console.WriteLine($"  firstOp: ({string.Join(", ", firstOp)})");
                Console.WriteLine($"   result: ({string.Join(", ", result)})");
                Console.WriteLine();
            }
        }
    }
}
