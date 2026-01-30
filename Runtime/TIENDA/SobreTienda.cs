using System.Collections.Generic;
using Bounds.Cofres;
using Bounds.Modulos.Cartas.Ilustradores;
using Bounds.Modulos.Cartas.Tinteros;
using Bounds.Persistencia;
using Bounds.Persistencia.Datos;
using UnityEngine;

namespace Bounds.Tienda {

	public abstract class SobreTienda : MonoBehaviour {

		public Coleccion coleccion;
		public string codigo;
		protected readonly int INDICE_ILUSTRACION = 0;
		protected readonly int INDICE_NOMBRE = 1;
		public ITintero tintero;
		public IlustradorDeCartas ilustrador1;


		void Start() {
			coleccion = new Coleccion(codigo, "");
			tintero = new TinteroBounds();
			ilustrador1.Inicializar();
			InicializarImagen();
		}


		protected void InicializarImagen() {
			EstablecerImagen(INDICE_ILUSTRACION, coleccion.emblema.cartaID);
		}


		protected void EstablecerTexto(int indice, string texto) {
			//Cartel componente = transform.GetChild(indice).GetComponentInChildren<Cartel>();
			//componente.setTexto(texto);
		}


		protected void EstablecerImagen(int indice, int cartaID) {
			GetComponent<ContenedorDeCartas>()?.Inicializar(ilustrador1, tintero, cartaID);
		}


		protected void EstablecerPosesion(int indice) {
			Cofre cofre = new Cofre();
			List<CartaColeccionBD> cartas = coleccion.GetListaCompleta();
			List<int> cartasID = new List<int>();
			foreach (var carta in cartas) {
				if (!cartasID.Contains(carta.cartaID))
					cartasID.Add(carta.cartaID);
			}
			int cartasObtenidas = cofre.GetCantidadCartasDiferentes(cartasID);
			int cartasTotales = cartas.Count;
			int porcentaje = (int)(((float)cartasObtenidas / cartasTotales) * 100);
			EstablecerTexto(indice, $"Posesión {cartasObtenidas}/{cartasTotales} ({porcentaje}%)");
		}


	}

}