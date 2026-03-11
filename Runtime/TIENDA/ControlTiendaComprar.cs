using Bounds.Cofres;
using Bounds.Modulos.Cartas.Ilustradores;
using Bounds.Modulos.Cartas.Persistencia;
using Bounds.Modulos.Cartas.Persistencia.Datos;
using Bounds.Musica;
using Bounds.Persistencia;
using Bounds.Persistencia.Parametros;
using Ging1991.Core;
using Ging1991.Core.Interfaces;
using Ging1991.Musica;
using Ging1991.Persistencia.Direcciones;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bounds.Tienda {

	public class ControlTiendaComprar : SingletonMonoBehaviour<ControlTiendaComprar> {

		public Configuracion configuracion;
		public Billetera billetera;
		public ParametrosControl parametrosControl;
		public DireccionRecursos carpetaColecciones;
		public GestorDeSobres gestorDeSobres;
		public string escenaAnterior;
		public MusicaDeFondo musicaDeFondo;
		public IProveedor<string, Sprite> selectorImagenes;
		public Cofre cofre;
		public GestorDeSonidos gestorDeSonidos;

		void Start() {
			parametrosControl.Inicializar();
			ParametrosEscena parametros = parametrosControl.parametros;

			configuracion = new(parametros.direcciones["CONFIGURACION"]);
			billetera = new(parametros.direcciones["BILLETERA"]);
			cofre = new(parametros.direcciones["COFRE"], parametros.direcciones["COFRE_RECURSOS"]);
			carpetaColecciones = new(parametros.direcciones["COLECCIONES"]);
			gestorDeSobres = new(parametros.direcciones["SOBRES"]);
			escenaAnterior = parametros.escenaPadre;
			musicaDeFondo.Inicializar(parametros.direcciones["MUSICA_DE_TIENDA"]);
			gestorDeSonidos.Inicializar(new DireccionRecursos(parametros.direcciones["SONIDOS"]));
			selectorImagenes = new IlustradorDeCartas(
				parametrosControl.parametros.direcciones["CARTAS_RECURSO"],
				parametrosControl.parametros.direcciones["CARTAS_DINAMICA"]
			);
			IProveedor<int, CartaBD> proveedorCartas = new LectorCartas(new DireccionRecursos(parametrosControl.parametros.direcciones["CARTAS_DATOS"]));
			foreach (var sobre in FindObjectsByType<SobreComprar>(
						 FindObjectsInactive.Include,
						 FindObjectsSortMode.None)) {
				sobre.Inicializar(proveedorCartas);
			}
		}

		public void PresionarAbrir() {
			SceneManager.LoadScene("TIENDA ABRIR");
		}

		public void PresionarVolver() {
			SceneManager.LoadScene(escenaAnterior);
		}

	}

}