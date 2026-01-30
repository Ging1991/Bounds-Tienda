using Bounds.Persistencia.Parametros;
using Ging1991.Persistencia.Direcciones;

namespace Bounds.Tienda {

	public class ParametrosControlTienda : ParametrosControl {

		public override void SetParametros() {
			parametros.direcciones["SOBRES"] = new DireccionDinamica("TIENDA", "SOBRES.json").Generar();
			parametros.direcciones["CONFIGURACION"] = new DireccionDinamica("CONFIGURACION", "CONFIGURACION.json").Generar();
			parametros.direcciones["COLECCIONES"] = "COLECCIONES";
			parametros.escenaPadre = "TEST";
		}

	}

}