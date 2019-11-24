using FsCheck;
using FsCheck.Xunit;
using System;
using Xunit;

namespace SFX.BitHack.CSharp.Tests
{
    [Trait("Category", "Unit")]
    public sealed class BitArrayAccessResultTests
    {
        #region Initialization
        [Property]
        public Property Initialization_works(BitArrayAccessError? error, int? value)
        {
            var sut = new BitArrayAccessResult<int>(error, value);
            return (error == sut.Error && value == sut.Value).ToProperty();
        }
        #endregion

        #region Deconstruction
        [Property]
        public Property Deconstruction_works(BitArrayAccessError? error, int? value)
        {
            var sut = new BitArrayAccessResult<int>(error, value);
            var (error_, value_) = sut;

            return (error == error_ && value == value_).ToProperty();
        }
        #endregion

        #region Conversion
        [Property]
        public Property Conversion_works(BitArrayAccessError? error, int? value)
        {
            var sut = new BitArrayAccessResult<int>(error, value);

            if (error.HasValue)
            {
                if (error.Value == BitArrayAccessError.IndexOutOfRange)
                {
                    try
                    {
                        int value_ = sut;
                    }
                    catch (IndexOutOfRangeException _)
                    {
                        return true.ToProperty();
                    }
                    catch
                    {
                        return false.ToProperty();
                    }
                    return false.ToProperty();
                }
                else
                {
                    try
                    {
                        int value_ = sut;
                    }
                    catch (Exception _)
                    {
                        return true.ToProperty();
                    }
                    return false.ToProperty();
                }
            }
            else
            {
                if (value.HasValue)
                {
                    int value_ = sut;
                    return (value_ == value).ToProperty();
                }
                else
                {
                    try
                    {
                        int value_ = sut;
                    }
                    catch (InvalidCastException _)
                    {
                        return true.ToProperty();
                    }
                    catch (Exception _)
                    {
                        return false.ToProperty();
                    }
                    return false.ToProperty();
                }
            }
        }
        #endregion
    }
}
