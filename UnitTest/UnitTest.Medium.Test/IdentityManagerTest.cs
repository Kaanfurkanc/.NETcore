namespace UnitTest.Medium.Test
{
    public class IdentityManagerTest
    {


        [Fact(Skip = "it is not necessary for now")]
        public void addIdentityNumberTr_WhenNumberIsNotElevenDigit_ThrowsException()
        {
            // Arrange
            var identityManager = new IdentityManager();
            const string invalidIdentityNumber = "123456789";

            // Act
            Action actual = () => identityManager.addIdentityNumberTr(invalidIdentityNumber);

            // Assert
            Assert.Throws<Exception>(actual);

        }
    }
}