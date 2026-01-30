using UnityEngine;
using System.Collections.Generic;
using Bounds.Persistencia;
using Bounds.Cofres;
using Bounds.Modulos.Cartas.Ilustradores;
using Bounds.Modulos.Cartas.Tinteros;
using Ging1991.Persistencia.Direcciones;
using Bounds.Persistencia.Parametros;
using UnityEngine.SceneManagement;

namespace Bounds.Tienda {

	public class ControlTiendaAbrir : MonoBehaviour {

		public GameObject objSobre;
		public List<GameObject> sobres = new List<GameObject>();
		private Cofre cofre;
		public IlustradorDeCartas ilustrador;

		public Configuracion configuracion;
		public ParametrosControl parametrosControl;
		public DireccionRecursos carpetaColecciones;
		public GestorDeSobres gestorDeSobres;

		void Start() {
			parametrosControl.Inicializar();
			ParametrosEscena parametros = parametrosControl.parametros;
			configuracion = new(parametros.direcciones["CONFIGURACION"]);
			carpetaColecciones = new(parametros.direcciones["COLECCIONES"]);
			gestorDeSobres = new(parametros.direcciones["SOBRES"]);

			cofre = new Cofre();
			ilustrador.Inicializar();
			ITintero tintero = new TinteroBounds();
			List<string> claves = new List<string>(){
				"COMPLETA100", "COMPLETA200", "COMPLETA300", "COMPLETA400", "COMPLETA500",
				"COMPLETA600", "ENERO2026", "ANTIGUOS", "BASICOS", "EQUIPOS",
				"HECHIZOS", "TRAMPAS", "AURAS", "EXPLOSION", "OCEANO",
				"OSCURIDAD", "BOSQUE", "TRUENO", "DIVINIDAD", "FAMILIA"
			};
			foreach (string clave in claves) {
				CrearSobre(new Coleccion(clave, carpetaColecciones.Generar(clave)), ilustrador, tintero);
			}
			Organizar();
		}


		private void CrearSobre(Coleccion coleccion, IlustradorDeCartas ilustrador, ITintero tintero) {
			int cantidad = gestorDeSobres.GetCantidad(coleccion.codigo);
			if (cantidad == 0)
				return;

			GameObject instancia = Instantiate(objSobre, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			instancia.name = "Sobre " + coleccion.codigo;
			GameObject contenedor = GameObject.Find("Sobres");
			instancia.transform.SetParent(contenedor.transform);
			instancia.transform.localScale = new Vector3(1, 1, 1);
			instancia.transform.localPosition = new Vector3(0, 0, 0);
			SobreAbrir componente = instancia.GetComponent<SobreAbrir>();
			componente.Iniciar(coleccion, ilustrador, tintero);
			this.sobres.Add(instancia);
		}


		private void Organizar() {
			int x = 0;
			int y = 0;
			int salto = 200;
			foreach (GameObject sobre in sobres) {
				sobre.transform.localPosition = new Vector3(x, y, 0);
				x += salto;
			}

			if (sobres.Count > 0) {
				sobres[0].transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
			}

		}


		public void Remover(GameObject sobre) {
			sobres.Remove(sobre);
			Organizar();
		}


		public void AgregarCarta(int cartaID, string imagen, string rareza) {
			cofre.AgregarCarta(new LineaReceta($"{cartaID}_{imagen}_{rareza}_1"));
		}


		public void Guardar() {
			cofre.Guardar();
		}


		public void PresionarComprar() {
			SceneManager.LoadScene("TIENDA COMPRAR");
		}

	}

}