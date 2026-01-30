using System.Collections.Generic;
using Bounds.Modulos.Cartas.Persistencia;
using Bounds.Modulos.Cartas.Persistencia.Datos;
using Bounds.Modulos.Cartas.Tinteros;
using Ging1991.Core.Interfaces;
using Ging1991.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Bounds.Tienda {

	public class ContenedorDeCartas : MonoBehaviour {

		public GameObject ilustracionOBJ;
		public List<GameObject> titulosOBJ;

		public void Inicializar(IIlustrador ilustrador, ITintero tintero, int cartaID, string imagen = "A") {
			string direccion = $"carta{cartaID}{imagen}";
			if (direccion.EndsWith("A")) {
				direccion = direccion[..^1];
			}

			ilustracionOBJ.GetComponent<Image>().sprite = ilustrador.GetElemento(direccion);
			DatosDeCartas.Instancia.Inicializar();
			CartaBD carta = DatosDeCartas.Instancia.lector.LeerDatos(cartaID);
			string clase = carta.clase != "CRIATURA" ? carta.clase : carta.datoCriatura.perfeccion;
			GetComponent<Marco>().SetColorRelleno(tintero.GetColor($"RELLENO_{clase}"));
			foreach (GameObject titulo in titulosOBJ) {
				titulo.GetComponent<MarcoConTexto>().SetColorRelleno(tintero.GetColor($"RELLENO_CLARO_{clase}"));
			}
		}

	}

}