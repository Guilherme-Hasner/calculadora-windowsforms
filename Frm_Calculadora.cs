using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculadora
{
    public partial class Frm_Calculadora : Form
    {
        bool power = false;
        string offScreen = "";
        string dgtd = "";
        bool dot = true; // Variável booleana que diz se pode-se inserir vírgula
        List<(bool num, bool op)> numeroOp = new List<(bool, bool)>(); //Diz se um caracter digitado é uma operação ou não
        /*           NÚMERO:     true, false
         *           OPERAÇÃO:   false, true
         *           VÍRGULA:    false, false
         *           NEGATIVO:   true, true                                 */
        List<(bool grau2, bool incremento)> operacao = new List<(bool, bool)>();
        /*SOMA:      false, true
         *SUBTRAÇÃO: (representada como soma)
         *MULTIPLC:  true, true
         *DIVISÃO:   true, false                                             */

        public Frm_Calculadora()
        {
            InitializeComponent();
        }

        private void Txb_display_TextChanged(object sender, EventArgs e)
        {

        }

        private void Btn_on_off_Click(object sender, EventArgs e)
        {
            if (power)
            {
                power = false;
                offScreen = Txb_display.Text;
                Txb_display.Text = "";
                Btn_on_off.BackColor = Color.RosyBrown;
            }
            else
            {
                power = true;
                Txb_display.Text = offScreen;
                Btn_on_off.BackColor = Color.Green;
            }
        }

        private void Btn_clear_Click(object sender, EventArgs e)
        {
            if (power)
            {
                clear();
            }
        }

        private void Btn_backspace_Click(object sender, EventArgs e)
        {
            int lastI = numeroOp.Count - 1;
            if (power && numeroOp.Count != 0)
            {
                if (numeroOp[lastI].num || !numeroOp[lastI].op) // Condicional que recebe qualquer caractere menos operação
                {
                    Txb_display.Text = Txb_display.Text.Remove(Txb_display.Text.Length - 1);
                    dgtd = dgtd.Remove(dgtd.Length - 1);
                    if (!numeroOp[lastI].num) { dot = true; } // Ao apagar uma vírgula ativa novamente a possibilidade de inserir uma
                    if (numeroOp[lastI].op) { Txb_display.Text = Txb_display.Text.Remove(Txb_display.Text.Length - 2); } // Ajuste para apagar 3 caracteres ao invés de um no caso de " - "
                }
                else // Remove 3 caracteres da tela no caso de operações e remove também a operação
                {
                    Txb_display.Text = Txb_display.Text.Remove(Txb_display.Text.Length - 3);
                    operacao.RemoveAt(operacao.Count - 1);
                }
                numeroOp.RemoveAt(lastI);
            }
        }

        private void Btn_equal_Click(object sender, EventArgs e)
        {
            if (power && numeroOp.Count != 0)
            {
                if (numeroOp[numeroOp.Count - 1] == (true, false)) // Só calcula se o último digito for um número
                {
                    bool negativeToOp = false; // Variável para dizer se um sinal negativo acompanha outra operação ou não
                    int contOp = 0, contNum = 0;
                    List<float> num = new List<float>();
                    string numText = "";
                    for (int i = 0; i < numeroOp.Count; i++) // Armazenar números e adiciona operações adicionais de soma se necessário
                    {
                        if (!numeroOp[i].op) // Se número ou vírgula
                        {
                            numText += dgtd[contNum++];
                            negativeToOp = true;
                        } else if (numeroOp[i] == (true, true)) // Se negativo
                        {
                            if (negativeToOp) // Se o símbolo negativo não for precedido por uma operação, uma operação de soma será adicionada nessa posição
                            {
                                operacao.Add((false, false));
                                for (int j = 1; j < (operacao.Count - contOp); j++)
                                {
                                    operacao[operacao.Count - j] = operacao[operacao.Count - j - 1];
                                }
                                operacao[contOp++] = (false, true);
                                num.Add(float.Parse(numText));
                                numText = "";
                            }
                            numText += dgtd[contNum++];
                        } else // Se operador (adição, divisão e multiplicação)
                        {
                            num.Add(float.Parse(numText));
                            numText = "";
                            contOp++;
                            negativeToOp = false;
                        }

                    }
                    num.Add(float.Parse(numText)); // Adiciona o último número após o fim do loop
                    for (int x = 0; x < operacao.Count; x++) // Realiza primeiro as operações de divisão e multiplicação em suas respectivas ordens
                    {
                        if (operacao[x].grau2)
                        {
                            if (operacao[x].incremento)
                            {
                                num[x + 1] *= num[x];
                            } else
                            {
                                num[x + 1] = num[x] / num[x + 1];
                            }
                            num[x] = 0;
                        }
                    }
                    float resultado = num.Sum(); // Soma o restante
                    // Os próximos passos são para zerar a calculadora e deixando-a apenas com o registro do resultado
                    clear();
                    if (resultado < 0)
                    {
                        Txb_display.Text = " - ";
                        dgtd = "-";
                        resultado *= -1;
                    }
                    string resultadoTxt = resultado.ToString();
                    for (int i = 0; i < resultadoTxt.Length; i++) { numeroOp.Add((true, false)); }
                    if (resultado % 1 != 0)
                    {
                        numeroOp[resultadoTxt.IndexOf(',')] = (false, false);
                        dot = false;
                    }
                    dgtd += resultadoTxt;
                    Txb_display.Text += resultadoTxt;
                }
                else { MessageBox.Show("Para apurar o resultado o último digito tem que ser um número", "ERRO - Não foi possível realizar a operação", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void Btn0_Click(object sender, EventArgs e)
        {
            if (power)
            {
                Txb_display.Text += "0";
                dgtd += "0";
                numeroOp.Add((true, false));
            }
        }

        private void Btn1_Click(object sender, EventArgs e)
        {
            if (power)
            {
                Txb_display.Text += "1";
                dgtd += "1";
                numeroOp.Add((true, false));
            }
        }

        private void Btn2_Click(object sender, EventArgs e)
        {
            if (power)
            {
                Txb_display.Text += "2";
                dgtd += "2";
                numeroOp.Add((true, false));
            }
        }

        private void Btn3_Click(object sender, EventArgs e)
        {
            if (power)
            {
                Txb_display.Text += "3";
                dgtd += "3";
                numeroOp.Add((true, false));
            }
        }

        private void Btn4_Click(object sender, EventArgs e)
        {
            if (power)
            {
                Txb_display.Text += "4";
                dgtd += "4";
                numeroOp.Add((true, false));
            }
        }

        private void Btn5_Click(object sender, EventArgs e)
        {
            if (power)
            {
                Txb_display.Text += "5";
                dgtd += "5";
                numeroOp.Add((true, false));
            }
        }

        private void Btn6_Click(object sender, EventArgs e)
        {
            if (power)
            {
                Txb_display.Text += "6";
                dgtd += "6";
                numeroOp.Add((true, false));
            }
        }

        private void Btn7_Click(object sender, EventArgs e)
        {
            if (power)
            {
                Txb_display.Text += "7";
                dgtd += "7";
                numeroOp.Add((true, false));
            }
        }

        private void Btn8_Click(object sender, EventArgs e)
        {
            if (power)
            {
                Txb_display.Text += "8";
                dgtd += "8";
                numeroOp.Add((true, false));
            }
        }

        private void Btn9_Click(object sender, EventArgs e)
        {
            if (power)
            {
                Txb_display.Text += "9";
                dgtd += "9";
                numeroOp.Add((true, false));
            }
        }

        private void Btn_dot_Click(object sender, EventArgs e)
        {
            if (power)
            {
                if (dot)
                {
                    if (numeroOp.Count == 0 || numeroOp[numeroOp.Count - 1].op)
                    {
                        Txb_display.Text += "0,";
                        
                    }
                    else
                    {
                        Txb_display.Text += ",";
                    }
                    dgtd += ",";
                    numeroOp.Add((false, false));
                    dot = false;
                } else { MessageBox.Show("Não é possível usar duas vírgulas em um número.", "ERRO - Não foi possível inserir a operação", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }
        
        private void Btn_sum_Click(object sender, EventArgs e)
        {
            if (power)
            {
                if (numeroOp.Count != 0)
                {
                    if (numeroOp.Count != 0 && numeroOp[numeroOp.Count - 1] == (true, false))
                    {
                        Txb_display.Text += " + ";
                        numeroOp.Add((false, true));
                        operacao.Add((false, true));
                        dot = true;
                    } else { MessageBox.Show("Não é possível usar duas operações seguidas ou no começo.", "ERRO - Não foi possível inserir a operação", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                } else { MessageBox.Show("Não é possível inserir uma operação antes de inserir um número.", "ERRO - Não foi possível inserir a operação", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void Btn_subtract_Click(object sender, EventArgs e)
        {
            if (power)
            {
                if ((numeroOp.Count == 0) || (numeroOp[numeroOp.Count - 1] == (true, false) || ((numeroOp[numeroOp.Count - 1] == (false, true) && operacao[operacao.Count - 1].grau2)))) // Se o último digito for um número ou uma operação diferente de "+"
                {
                    Txb_display.Text += " - ";
                    dgtd += "-";
                    numeroOp.Add((true, true));
                    dot = true;
                } else { MessageBox.Show("Não é possível usar duas operações seguidas.", "ERRO - Não foi possível inserir a operação", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void Btn_multiply_Click(object sender, EventArgs e)
        {
            if (power)
            {
                if (numeroOp.Count != 0)
                {
                    if (numeroOp[numeroOp.Count - 1] == (true, false))
                    {
                        Txb_display.Text += " x ";
                        numeroOp.Add((false, true));
                        operacao.Add((true, true));
                        dot = true;
                    } else { MessageBox.Show("Não é possível usar duas operações seguidas ou no começo.", "ERRO - Não foi possível inserir a operação", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                } else { MessageBox.Show("Não é possível inserir uma operação antes de inserir um número.", "ERRO - Não foi possível inserir a operação", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void Btn_division_Click(object sender, EventArgs e)
        {
            if (power)
            {
                if (numeroOp.Count != 0)
                {
                    if (numeroOp[numeroOp.Count - 1] == (true, false))
                    {
                        Txb_display.Text += " / ";
                        numeroOp.Add((false, true));
                        operacao.Add((true, false));
                        dot = true;
                    } else { MessageBox.Show("Não é possível usar duas operações seguidas ou no começo.", "ERRO - Não foi possível inserir a operação", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                } else { MessageBox.Show("Não é possível inserir uma operação antes de inserir um número.", "ERRO - Não foi possível inserir a operação", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void Btn_info_Click(object sender, EventArgs e)
        {
            if (power)
            {
                MessageBox.Show("Essa é uma calculadora básica porém já passou por uma rodada de testes, se apresentar algum defeito por favor entre em contato com nossos desenvolvedores.", "Calculadora V02", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void clear()
        {
            Txb_display.Clear();
            Txb_display.Text += "";
            offScreen = "";
            numeroOp.Clear();
            operacao.Clear();
            dgtd = "";
            dot = true;
        }
    }
}