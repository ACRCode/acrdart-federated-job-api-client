using JetBrains.Annotations;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Acr.Dart.FederatedNetwork.Api.Client
{
    /// <summary>
    /// Contains a set of Assertion routines to enforce contracts (preconditions).
    /// </summary>
    internal static class Assert
    {
        /// <summary>
        /// Verifies that a given value is not <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="value">Value to verify</param>
        /// <param name="name">Value name</param>
        /// <exception cref="ArgumentNullException">If value is <see langword="null"/></exception>
        [AssertionMethod]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Documentation", "Wintellect010:ExceptionDocumentationMissingAnalyzer", Justification = "Bug in the analyzer")]
        public static void NotNull<T>(
            [AssertionCondition(AssertionConditionType.IS_NOT_NULL), NoEnumeration] T value,
            [InvokerParameterName] string name) where T : class
        {
            if (value == null) throw new ArgumentNullException(name);
        }

        /// <summary>
        /// Verifies that a given precondition is satisfied.
        /// </summary>
        /// <param name="condition">Condition to verify</param>
        /// <param name="errorMessage">Error message to use when the condition check failed.</param>
        /// <exception cref="InvalidOperationException">If condition is not satisfied, i.e. is <see langword="false"/>.</exception>
        [AssertionMethod]
        [SuppressMessage("Documentation", "Wintellect010:ExceptionDocumentationMissingAnalyzer", Justification = "Bug in the analyzer")]
        public static void That(
            [AssertionCondition(AssertionConditionType.IS_TRUE)] bool condition,
            [CanBeNull] string errorMessage)
        {
            if (!condition)
            {
                throw new InvalidOperationException(errorMessage ?? "Precondition failed.");
            }
        }
    }
}