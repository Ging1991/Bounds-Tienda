using Bounds.Persistencia.Parametros;
using Ging1991.Persistencia.Direcciones;

namespace Bounds.Tienda {

	public class ParametrosControlTienda : ParametrosControl {

		public override void SetParametros() {
			parametros.direcciones["MUSICA_AMBIENTAL"] = "Sonidos/Ambiente";
			parametros.direcciones["COLORES"] = new DireccionRecursos("Configuracion", "COLORES").Generar();
			parametros.direcciones["SISTEMA"] = new DireccionRecursos("Configuracion", "IDIOMA").Generar();

			parametros.direcciones["SOBRES"] = new DireccionDinamica("TIENDA", "SOBRES.json").Generar();
			parametros.direcciones["CONFIGURACION"] = new DireccionDinamica("CONFIGURACION", "CONFIGURACION.json").Generar();
			parametros.direcciones["BILLETERA"] = new DireccionDinamica("CONFIGURACION", "BILLETERA.json").Generar();
			parametros.direcciones["COLECCIONES"] = "COLECCIONES";
			parametros.escenaPadre = "TEST";
			parametros.direcciones["MUSICA_DE_FONDO"] = new DireccionRecursos("Musica", "Fondo").Generar();
			parametros.direcciones["MUSICA_DE_TIENDA"] = new DireccionRecursos("Musica", "TIENDA").Generar();
			parametros.direcciones["CARTAS_RECURSO"] = "Cartas/Imagenes";
			parametros.direcciones["CARTAS_DATOS"] = "Cartas/Datos";
			parametros.direcciones["SONIDOS"] = "Sonidos";
			parametros.direcciones["CARTAS_DINAMICA"] = "IMAGENES/Cartas/Imagenes";
			parametros.direcciones["COFRE_RECURSOS"] = new DireccionRecursos("MAZOS", "COFRE").Generar();
			parametros.direcciones["COFRE"] = new DireccionDinamica("CONFIGURACION", "COFRE.json").Generar();

		}

	}

}