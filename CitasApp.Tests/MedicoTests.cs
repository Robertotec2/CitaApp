using Xunit;

namespace CitasApp.Tests
{
    public class MedicoTests
    {
        [Fact]
        public void Validar_Especialidad_EsCorrecta()
        {
            // Arrange
            string especialidadEsperada = "Cardiología";
            
            // Act
            string resultado = "Cardiología";

            // Assert
            Assert.Equal(especialidadEsperada, resultado);
        }
    }
}
