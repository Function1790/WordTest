using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Word
{
    public string Eng = "";
    public string Kor = "";

    public Word(){}

    public Word(string Eng, string Kor)
    {
        this.Eng = Eng;
        this.Kor = Kor;
    }

    public Word(Word value)
    {
        this.Eng = value.Eng;
        this.Kor = value.Kor;
    }

    public void Set(string Eng, string Kor)
    {
        this.Eng = Eng;
        this.Kor = Kor;
    }

    public void Set(Word value)
    {
        this.Set(value.Eng, value.Kor);
    }

    public Word deepCopy()
    {
        Word word = new Word();
        word.Eng = Eng;
        word.Kor = Kor;
        return word;
    }
}