using Bounds.Persistencia;
using Bounds.Persistencia.Parametros;
using Ging1991.Persistencia.Direcciones;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bounds.Tienda {

	public class ControlTiendaComprar : MonoBehaviour {

		public Configuracion configuracion;
		public ParametrosControl parametrosControl;
		public DireccionRecursos carpetaColecciones;
		public GestorDeSobres gestorDeSobres;
		public string escenaAnterior;

		void Awake() {
			parametrosControl.Inicializar();
			ParametrosEscena parametros = parametrosControl.parametros;
			configuracion = new(parametros.direcciones["CONFIGURACION"]);
			carpetaColecciones = new(parametros.direcciones["COLECCIONES"]);
			gestorDeSobres = new(parametros.direcciones["SOBRES"]);
			escenaAnterior = parametros.escenaPadre;
		}

		public void PresionarAbrir() {
			SceneManager.LoadScene("TIENDA ABRIR");
		}

		public void PresionarVolver() {
			SceneManager.LoadScene(escenaAnterior);
		}

	}

}