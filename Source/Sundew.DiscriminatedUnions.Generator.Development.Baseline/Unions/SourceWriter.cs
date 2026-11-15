// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceWriter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Unions;

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// Writes lines of source code and keeps track of the indentation.
/// A line that is written as an interpolated string is appended piece by piece, so no string is created for the line itself.
/// </summary>
internal sealed class SourceWriter
{
    private const string Indentation = "    ";
    private const string NewLine = "\r\n";
    private const string BlockStart = "{";
    private const string BlockEnd = "}";
    private const int MaximumCachedCapacity = 64 * 1024;

    // Generators run on several threads, and each of them gets to reuse the buffer of the source it wrote last.
    [ThreadStatic]
    private static StringBuilder? cachedStringBuilder;

    private readonly StringBuilder stringBuilder;
    private int indentationLevel;

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceWriter"/> class.
    /// </summary>
    public SourceWriter()
    {
        this.stringBuilder = cachedStringBuilder ?? new StringBuilder();
        cachedStringBuilder = null;
    }

    /// <summary>
    /// Writes an empty line.
    /// </summary>
    /// <returns>The source writer.</returns>
    public SourceWriter Line()
    {
        this.stringBuilder.Append(NewLine);
        return this;
    }

    /// <summary>
    /// Writes the text as an indented line.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>The source writer.</returns>
    public SourceWriter Line(string text)
    {
        this.AppendIndentation();
        this.stringBuilder.Append(text).Append(NewLine);
        return this;
    }

    /// <summary>
    /// Writes the interpolated text as an indented line.
    /// </summary>
    /// <param name="text">The text, which has already been appended by the time this is called.</param>
    /// <returns>The source writer.</returns>
    public SourceWriter Line([InterpolatedStringHandlerArgument("")] ref LineText text)
    {
        this.stringBuilder.Append(NewLine);
        return this;
    }

    /// <summary>
    /// Starts a block and indents the lines that follow.
    /// </summary>
    /// <returns>The source writer.</returns>
    public SourceWriter OpenBlock()
    {
        this.Line(BlockStart);
        this.indentationLevel++;
        return this;
    }

    /// <summary>
    /// Ends the current block. A blank line right before the end of the block is removed.
    /// </summary>
    /// <returns>The source writer.</returns>
    public SourceWriter CloseBlock()
    {
        var length = this.stringBuilder.Length;
        var blankLineLength = NewLine.Length * 2;
        if (length >= blankLineLength && this.EndsWithNewLine(length) && this.EndsWithNewLine(length - NewLine.Length))
        {
            this.stringBuilder.Length -= NewLine.Length;
        }

        this.indentationLevel--;
        return this.Line(BlockEnd);
    }

    /// <summary>
    /// Indents the lines that follow.
    /// </summary>
    /// <returns>The source writer.</returns>
    public SourceWriter Indent()
    {
        this.indentationLevel++;
        return this;
    }

    /// <summary>
    /// Removes one level of indentation from the lines that follow.
    /// </summary>
    /// <returns>The source writer.</returns>
    public SourceWriter Unindent()
    {
        this.indentationLevel--;
        return this;
    }

    /// <summary>
    /// Gets the source that was written and hands the buffer on to the next source writer. Nothing can be written after this.
    /// </summary>
    /// <returns>The source.</returns>
    public override string ToString()
    {
        var source = this.stringBuilder.ToString();
        if (this.stringBuilder.Capacity <= MaximumCachedCapacity)
        {
            this.stringBuilder.Clear();
            cachedStringBuilder = this.stringBuilder;
        }

        return source;
    }

    private void AppendIndentation()
    {
        for (var i = 0; i < this.indentationLevel; i++)
        {
            this.stringBuilder.Append(Indentation);
        }
    }

    private bool EndsWithNewLine(int length)
    {
        return this.stringBuilder[length - 2] == NewLine[0] && this.stringBuilder[length - 1] == NewLine[1];
    }

    /// <summary>
    /// Appends the parts of an interpolated string to a source writer as the line it is writing.
    /// The compiler uses it for <see cref="SourceWriter.Line(ref LineText)"/>, when the argument is an interpolated string that is not a constant.
    /// </summary>
    [InterpolatedStringHandler]
    public readonly ref struct LineText
    {
        private readonly StringBuilder stringBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineText"/> struct and starts the line.
        /// </summary>
        /// <param name="literalLength">The number of characters outside the holes of the interpolated string.</param>
        /// <param name="formattedCount">The number of holes in the interpolated string.</param>
        /// <param name="sourceWriter">The source writer that writes the line.</param>
        public LineText(int literalLength, int formattedCount, SourceWriter sourceWriter)
        {
            sourceWriter.AppendIndentation();
            this.stringBuilder = sourceWriter.stringBuilder;
        }

        /// <summary>
        /// Appends a part of the interpolated string that is outside its holes.
        /// </summary>
        /// <param name="value">The text.</param>
        public void AppendLiteral(string value)
        {
            this.stringBuilder.Append(value);
        }

        /// <summary>
        /// Appends the text in a hole of the interpolated string.
        /// </summary>
        /// <param name="value">The text.</param>
        public void AppendFormatted(string? value)
        {
            this.stringBuilder.Append(value);
        }

        /// <summary>
        /// Appends the character in a hole of the interpolated string.
        /// </summary>
        /// <param name="value">The character.</param>
        public void AppendFormatted(char value)
        {
            this.stringBuilder.Append(value);
        }

        /// <summary>
        /// Appends the number in a hole of the interpolated string, as it is written in source code.
        /// </summary>
        /// <param name="value">The number.</param>
        public void AppendFormatted(int value)
        {
            this.stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
