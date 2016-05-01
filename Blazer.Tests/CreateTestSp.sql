-- Creates a Stored Procedure for testing purposes.
-- This SP tests output- and return parameters.

USE [AdventureWorks]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.SpBlazerTest'))
   exec('CREATE PROCEDURE [dbo].[SpBlazerTest] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE [dbo].[SpBlazerTest]
	@x int,
	@y int,
	@i int output,
	@msg VARCHAR(20) output,
	@nullVal VARCHAR(20) output
AS
BEGIN
	SET NOCOUNT ON

	SET @i = @i + 1
	SET @msg = 'Hello Blazer!'
	SET @nullVal = NULL

	RETURN @x + @y
END