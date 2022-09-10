namespace System.Refactive;

public delegate void RefAction<T>(ref T item);
public delegate void RefAction<T1, T2>(ref T1 arg1, ref T2 arg2);