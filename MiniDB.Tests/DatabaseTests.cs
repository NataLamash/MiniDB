using System;
using System.Collections.Generic;
using System.Text;
using MiniDB.Core.DataTypes;
using MiniDB.Core.Models;
using MiniDB.Core.Services;

namespace MiniDB.Tests;

public class DatabaseTests
{
    [Fact]
    public void IntegerType_ValidInteger_ReturnsTrue()
    {
        // Arrange
        var type = new IntegerType();

        // Act
        bool result = type.Validate("123");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ColorType_InvalidColor_ReturnsFalse()
    {
        // Arrange
        var type = new ColorType();

        // Act
        bool result = type.Validate("#GG0000");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AddRow_ValidRow_AddsRowToTable()
    {
        // Arrange
        var table = new Table("Students");

        table.Columns.Add(new Column("Id", "integer"));
        table.Columns.Add(new Column("Name", "string"));

        var row = new Row(new[]
        {
            "1",
            "Anna"
        });

        // Act
        table.AddRow(row);

        // Assert
        Assert.Single(table.Rows);
        Assert.Equal("Anna", table.Rows[0].Values[1]);
    }

    [Fact]
    public void RenameColumn_ChangesColumnName()
    {
        // Arrange
        var table = new Table("Students");

        table.Columns.Add(new Column("Id", "integer"));
        table.Columns.Add(new Column("Name", "string"));

        var service = new ColumnSchemaService();

        // Act
        service.RenameColumn(
            table,
            "Name",
            "FullName");

        // Assert
        Assert.Equal(
            "FullName",
            table.Columns[1].Name);
    }

    [Fact]
    public void MoveColumn_MovesColumnAndCorrespondingRowValues()
    {
        // Arrange
        var table = new Table("Students");

        table.Columns.Add(new Column("Id", "integer"));
        table.Columns.Add(new Column("Name", "string"));
        table.Columns.Add(new Column("Grade", "real"));

        table.Rows.Add(
            new Row(new[]
            {
                "1",
                "Anna",
                "95.5"
            }));

        var service = new ColumnSchemaService();

        // Act
        service.MoveColumn(
            table,
            oldIndex: 2,
            newIndex: 0);

        // Assert
        Assert.Equal(
            "Grade",
            table.Columns[0].Name);

        Assert.Equal(
            "95.5",
            table.Rows[0].Values[0]);

        Assert.Equal(
            "Id",
            table.Columns[1].Name);

        Assert.Equal(
            "1",
            table.Rows[0].Values[1]);

        Assert.Equal(
            "Name",
            table.Columns[2].Name);

        Assert.Equal(
            "Anna",
            table.Rows[0].Values[2]);
    }

    [Fact]
    public void ColorIntervalType_ValidInterval_ReturnsTrue()
    {
        var type = new ColorIntervalType();

        bool result =
            type.Validate("#000000..#FFFFFF");

        Assert.True(result);
    }
}
