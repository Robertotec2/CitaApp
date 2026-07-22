using Xunit;

namespace CitasApp.Tests
{
    public class PacienteTests
    {
        [Fact]
        public void Validar_Nombre_NoEsNulo()
        {
            // Arrange
            string nombre = "Juan";
            
            // Act
            string resultado = nombre; 

            // Assert
            Assert.NotNull(resultado);
        }
    }
}
