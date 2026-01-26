using DocumentFormat.OpenXml.Office2010.CustomUI;

public class CreateOrderValidatorTest
{
    private readonly CreateOrderValidator _validator;
    public CreateOrderValidatorTest()
    {
        
           _validator = new CreateOrderValidator();

    }
    [Fact]
    public void Validator_AboutAmount()
    {
        var Command = new CreateOrderCommand(Guid.NewGuid(),0, null);
        var result = _validator.TestValidate(Command);
        result.ShouldHaveValidationErrorFor(x => x.TotalAmount)
         .WithErrorMessage("Tổng giá trị Order phải lớn hơn 0");

    }

}