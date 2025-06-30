using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

abstract class Handler
{
    protected Handler Next;
    public void SetNext(Handler next) => Next = next;
    public abstract void HandleRequest(SupportRequest request);
}

class Technician : Handler
{
    public override void HandleRequest(SupportRequest request)
    {
        if (request.Complexity <= 1)
            Console.WriteLine("Технік обробив заявку: " + request.Description);
        else
            Next?.HandleRequest(request);
    }
}

class Manager : Handler
{
    public override void HandleRequest(SupportRequest request)
    {
        if (request.Complexity <= 2)
            Console.WriteLine("Менеджер обробив заявку: " + request.Description);
        else
            Next?.HandleRequest(request);
    }
}

class Director : Handler
{
    public override void HandleRequest(SupportRequest request)
    {
        Console.WriteLine("Директор обробив складну заявку: " + request.Description);
    }
}

class SupportRequest
{
    public string Description { get; }
    public int Complexity { get; }

    public SupportRequest(string desc, int complexity)
    {
        Description = desc;
        Complexity = complexity;
    }
}

class RequestCollection : IEnumerable<SupportRequest>
{
    private List<SupportRequest> requests = new();
    public void Add(SupportRequest req) => requests.Add(req);
    public IEnumerator<SupportRequest> GetEnumerator() => requests.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

interface ICommand
{
    void Execute();
}

class AcceptCommand : ICommand
{
    private SupportRequest _request;
    public AcceptCommand(SupportRequest req) => _request = req;
    public void Execute() => Console.WriteLine("Заявку прийнято: " + _request.Description);
}

class RejectCommand : ICommand
{
    private SupportRequest _request;
    public RejectCommand(SupportRequest req) => _request = req;
    public void Execute() => Console.WriteLine("Заявку відхилено: " + _request.Description);
}

class DeleteCommand : ICommand
{
    private SupportRequest _request;
    public DeleteCommand(SupportRequest req) => _request = req;
    public void Execute() => Console.WriteLine("Заявку видалено: " + _request.Description);
}

class Program
{
    static void Main()
    {
        Handler tech = new Technician();
        Handler manager = new Manager();
        Handler director = new Director();

        tech.SetNext(manager);
        manager.SetNext(director);

        var requests = new RequestCollection();
        requests.Add(new SupportRequest("Не вмикається ПК", 1));
        requests.Add(new SupportRequest("Сервер не працює", 3));

        foreach (var req in requests)
        {
            tech.HandleRequest(req);
        }

        Console.WriteLine("\n--- Команди ---");
        ICommand accept = new AcceptCommand(requests.First());
        ICommand reject = new RejectCommand(requests.Last());
        accept.Execute();
        reject.Execute();
    }
}
